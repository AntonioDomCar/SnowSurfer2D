using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private ParticleSystem crashEffect; // Particle system for crash effect
   

    void OnTriggerEnter2D(Collider2D other)
    {
        int layerMask = LayerMask.NameToLayer("Floor");
        if (other.gameObject.layer == layerMask)
        {
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

