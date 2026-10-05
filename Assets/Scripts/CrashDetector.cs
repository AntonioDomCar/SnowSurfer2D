using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private ParticleSystem crashEffect; // Particle system for crash effect
    private PlayerController playerController; // Reference to the PlayerController script

   void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int layerMask = LayerMask.NameToLayer("Floor");
        if (other.gameObject.layer == layerMask)
        {
            playerController.CanControlPlayer = false; // Disable player control
            Debug.Log("Player has crashed!"); 
            crashEffect.Play(); // Play the crash effect particles  
        }
        Invoke(nameof(ReloadScene), 1f); // Reload the scene after 2 seconds
    }

        void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

