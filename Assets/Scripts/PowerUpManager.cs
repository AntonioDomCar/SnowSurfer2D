using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] private PowerUpScriptableObjectScript powerUpData; // Reference to the PowerUpScriptableObjectScript

    PlayerController playerController; // Reference to the PlayerController script
    SpriteRenderer powerUpSpriteRenderer; // Reference to the SpriteRenderer for the power-up visual representation

    float timeleft;

    /// <summary>
    /// Initializes the PowerUpManager by finding the PlayerController and setting up the power-up sprite renderer. It also initializes the time left for the power-up effect based on the data from the PowerUpScriptableObjectScript.
    /// </summary>
    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        powerUpSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        timeleft = powerUpData.TimeLimit;
    }

    private void Update()
    {
        CountDownPowerUpTime(); // Update the countdown for the power-up effect
    }

    /// <summary>
    /// Updates the power-up effect countdown. If the time left for the power-up effect is greater than zero, it decreases the time left. Once the time left reaches zero, it re-enables player control and resets the time left for the next power-up activation.
    /// </summary>
    private void CountDownPowerUpTime()
    {
        if (powerUpSpriteRenderer.enabled == false)
        {
            if (timeleft > 0)
            {
                timeleft -= Time.deltaTime;
            }
            else
            {
                playerController.deactivatePowerUp(powerUpData); // Re-enable player control after the power-up effect ends
            }
            
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && powerUpSpriteRenderer.enabled) // Check if the player collides with the power-up and if the power-up is visible
        {
            powerUpSpriteRenderer.enabled = false; // Hide the power-up sprite
            
            playerController.ApplyPowerUp(powerUpData); // Apply the power-up effect to the player
        }
    }


}
