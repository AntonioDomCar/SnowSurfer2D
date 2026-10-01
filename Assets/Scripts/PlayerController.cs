using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float torqueAmount = 1f; // velocidad de rotacion



    InputAction moveAction; // informacion entrada salida
    Vector2 moveInput; // informacion move input
    Rigidbody2D rb; // referencia al rigidbody


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
       PlayerTorque();

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
}
