using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento y Rotación")]
    [SerializeField] private float torqueAmount = 1f; // velocidad de rotacion
    [SerializeField] private float boostSpeed = 35f; // velocidad de impulso

    [Header("Mecánica de Salto")]
    [SerializeField] public float jumpForce = 10f; // fuerza del salto
    private bool isGrounded = false; // control de si está en el suelo

    [Header("Efectos y Managers")]
    [SerializeField] private ParticleSystem snowEffect; // Particle system for boost effect
    [SerializeField] private ScoreManager scoreManager; // Reference to the ScoreManager script

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
    }

    // Update is called once per frame
    void Update()
    {
        if(!canControlPlayer) return;  // If the player cannot control, exit the method
        
        PlayerTorque();
        BoostPlayer();
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
        // Get the current rotation of the player
        float currentRotation = transform.eulerAngles.z; 

        // Calculate the change in rotation since the last frame
        totalRotation += Mathf.DeltaAngle(previousRotation, currentRotation); 

        if(Mathf.Abs(totalRotation) > 340)
        {
            flipCount++;
            scoreManager.AddScore(flipCount * 100); // Update the score in the ScoreManager
            totalRotation = 0; // Reset total rotation after a flip is counted
        }

        previousRotation = currentRotation; // Update previous rotation for the next frame
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
            isGrounded = true; // El jugador tocó el suelo
            snowEffect.Play(); // Play the snow effect particles
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
}