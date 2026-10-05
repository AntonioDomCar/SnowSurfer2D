using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float torqueAmount = 1f; // velocidad de rotacion
    [SerializeField] private float boostSpeed = 35f; // velocidad de impulso
    [SerializeField] private ParticleSystem snowEffect; // Particle system for boost effect

    private bool canControlPlayer = true; // Flag to control player input
    public bool CanControlPlayer { get => canControlPlayer; set => canControlPlayer = value; }
    
    SurfaceEffector2D surfaceEffector; // referencia al surface effector

    float baseSpeed; // velocidad base del surface effector

    InputAction moveAction; // informacion entrada salida
    Vector2 moveInput; // informacion move input
    Rigidbody2D rb; // referencia al rigidbody

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        //Increase the player's speed based on the vertical input from the user.
        //Surface efector speed is increased to bootspeed the player.
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
            Debug.Log("Player is ont the floor!");
            snowEffect.Play(); // Play the snow effect particles
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            Debug.Log("Player is off the floor!");
            snowEffect.Stop(); // Stop the snow effect particles
        }
    }
}
