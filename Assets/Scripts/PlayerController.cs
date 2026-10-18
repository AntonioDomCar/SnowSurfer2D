using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento y Rotación")]
    [SerializeField] private float torqueAmount = 1f; // velocidad de rotacion
    [SerializeField] private float boostSpeed = 35f; // velocidad de impulso

    [Header("Mecánica de Salto")]
    [SerializeField] public float jumpForce = 10f; // fuerza del salto
    private bool isGrounded = false; // control de si está en el suelo

    [Header("Recompensa por Flips")]
    [SerializeField] private float speedMultiplierBase = 1.07f; // Multiplicador base exponencial
    private Coroutine speedBoostCoroutine;
    private bool isBoostingFromFlip = false; // Controla si el impulso por flip está activo
    private int flipsInCurrentAirTime = 0; // Control de flips en el salto actual

    [Header("Mecánica de Turbo (Boost)")]
    [SerializeField] private float maxTurbo = 100f; // Capacidad máxima del turbo
    [SerializeField] private float currentTurbo = 0f; // Turbo actual
    [SerializeField] private float turboGainPerFlip = 35f; // Cuánto turbo suma cada flip
    [SerializeField] private float turboBoostSpeed = 50f; // Velocidad extrema al activar el turbo

    private bool isTurboActive = false;

    [Header("Efectos y Managers")]
    [SerializeField] private ParticleSystem snowEffect; // Particle system for boost effect
    [SerializeField] private ScoreManager scoreManager; // Reference to the ScoreManager script

    [SerializeField] private Slider turboSlider; // Referencia a la barra UI

    private bool canControlPlayer = true; // Flag to control player input
    public bool CanControlPlayer { get => canControlPlayer; set => canControlPlayer = value; }
    
    SurfaceEffector2D surfaceEffector; // referencia al surface effector

    float baseSpeed; // velocidad base del surface effector
    float previousRotation; // rotacion previa del jugador
    float totalRotation; // rotacion total del jugador
    int flipCount; // contador de flips del jugador

    int activePowerUpsCount; // contador de power-ups activos

    InputAction moveAction; // informacion entrada salida
    Vector2 moveInput; // informacion move input
    Rigidbody2D rb; // referencia al rigidbody

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);

        Transform characters = transform.Find("Characters");

        if (characters == null)
        {
            Debug.LogError("No se encuentra Characters.");
            return;
        }

        // Desactivar todos los personajes primero
        foreach (Transform character in characters)
        {
            character.gameObject.SetActive(false);
        }

        // Buscar el personaje por su nombre
        string characterName = "Character" + selectedCharacter;
        Transform selected = characters.Find(characterName);

        if (selected != null)
        {
            selected.gameObject.SetActive(true);
            Debug.Log("Personaje activado: " + characterName);
        }
        else
        {
            Debug.LogError("No se encontró " + characterName);
        }

        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
        surfaceEffector = FindAnyObjectByType<SurfaceEffector2D>();
        baseSpeed = surfaceEffector.speed;

        if (turboSlider != null)
        {
            turboSlider.maxValue = maxTurbo;
            turboSlider.value = currentTurbo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!canControlPlayer) return;  // If the player cannot control, exit the method
        
        PlayerTorque();
        HandleTurbo();
        JumpPlayer(); 
        CalculateFlips();
    }

    /// <summary>
    /// Gestiona el salto del personaje utilizando el Input System y la fuerza vertical.
    /// </summary>
    void JumpPlayer()
    {
        // Detecta impulso hacia arriba en el mando/teclado (flecha arriba / W / Espacio) cuando está en el suelo
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false; // Evita saltos infinitos mientras despega
        }
    }

    /// <summary>
    /// Calculate the number of flips the player has made based on their rotation.
    /// This method can be used to trigger events or effects based on the number of flips.
    /// </summary>
    private void CalculateFlips()
    {
        float currentRotation = transform.eulerAngles.z; 
        totalRotation += Mathf.DeltaAngle(previousRotation, currentRotation); 

        if(Mathf.Abs(totalRotation) > 340)
        {
            flipCount++;
            flipsInCurrentAirTime++; 
            scoreManager.AddScore(flipCount * 100); 

            // Sumar turbo al completar un flip (con un tope máximo)
            currentTurbo = Mathf.Clamp(currentTurbo + turboGainPerFlip, 0f, maxTurbo);

            // Actualizar la barra visual
            if (turboSlider != null) turboSlider.value = currentTurbo;

            totalRotation = 0; 
        }

        previousRotation = currentRotation; 
    }

    /// <summary>
    /// Aplica torque al jugador basado en la entrada horizontal del usuario.
    /// </summary>
    void PlayerTorque()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput.x < 0f)
        {
            rb.AddTorque(torqueAmount);
        }
        else if (moveInput.x > 0f)
        {
            rb.AddTorque(-torqueAmount);
        }
    }

    void BoostPlayer()
    {

        // Si el impulso temporal por flip está activo, no sobrescribir la velocidad
        if (isBoostingFromFlip) return;
        // Increase the player's speed based on the vertical input from the user.
        // Surface effector speed is increased to boostspeed the player.
        if (moveInput.y > 0f)
        {
            surfaceEffector.speed = boostSpeed; // Increase speed when moving up
        }
        else
        {
            surfaceEffector.speed = baseSpeed; // Default speed when not moving up
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            Debug.Log("Player is on the floor!");
            isGrounded = true; 
            snowEffect.Play(); 

            // Si completó al menos un flip en el aire, aplica velocidad exponencial al aterrizar
            if (flipsInCurrentAirTime > 0)
            {
                // Cálculo exponencial: base elevada al número de flips (ej: 1.3^1, 1.3^2, 1.3^3...)
                float exponentialMultiplier = Mathf.Pow(speedMultiplierBase, flipsInCurrentAirTime);
                float boostedTargetSpeed = boostSpeed * exponentialMultiplier;

                // Inicia o reinicia la corrutina del boost temporal de 1 segundo
                if (speedBoostCoroutine != null)
                {
                    StopCoroutine(speedBoostCoroutine);
                }
                speedBoostCoroutine = StartCoroutine(ApplyTemporarySpeedBoost(boostedTargetSpeed, 0.5f));

                // Reinicia el contador para el próximo salto
                flipsInCurrentAirTime = 0;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            Debug.Log("Player is off the floor!");
            isGrounded = false; // El jugador despegó del suelo
            snowEffect.Stop(); // Stop the snow effect particles
        }
    }

    public void ApplyPowerUp(PowerUpScriptableObjectScript powerUpData)
    {
        activePowerUpsCount++;

        if (powerUpData.PowerUpType == "Speed")
        {
            baseSpeed += powerUpData.PowerUpValue; // Increase the base speed of the player
            boostSpeed += powerUpData.PowerUpValue; // Increase the boost speed of the player
        }
    }

    public void deactivatePowerUp(PowerUpScriptableObjectScript powerUpData)
    {
        activePowerUpsCount--;

        if(activePowerUpsCount == 0)
        {
            if (powerUpData.PowerUpType == "Speed")
            {
                baseSpeed -= powerUpData.PowerUpValue; // Decrease the base speed of the player
                boostSpeed -= powerUpData.PowerUpValue; // Decrease the boost speed of the player
            }
        }    
    }

    private System.Collections.IEnumerator ApplyTemporarySpeedBoost(float targetSpeed, float duration)
    {
        isBoostingFromFlip = true;
        surfaceEffector.speed = targetSpeed;

        yield return new WaitForSeconds(duration);

        isBoostingFromFlip = false;
        surfaceEffector.speed = baseSpeed; // <--- Restaura a la velocidad base
    }

    void HandleTurbo()
{
    if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed && currentTurbo > 0f)
    {
        isTurboActive = true;
        surfaceEffector.speed = turboBoostSpeed;

        currentTurbo -= Time.deltaTime * 40f;
        if (currentTurbo < 0f) currentTurbo = 0f;

        // Actualizar la barra visual mientras se gasta
        if (turboSlider != null) turboSlider.value = currentTurbo;
    }
    else if (isTurboActive)
    {
        isTurboActive = false;
        if (!isBoostingFromFlip)
        {
            surfaceEffector.speed = baseSpeed; // <--- Vuelve a la velocidad base al terminar el turbo
        }
    }
}
}