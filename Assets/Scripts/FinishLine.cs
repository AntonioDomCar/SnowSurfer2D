using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 1f; // Delay before reloading the scene
    [SerializeField] private ParticleSystem finishEffect; // Particle system for finish effect

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has crossed the finish line!");
            finishEffect.Play(); // Play the finish effect particles
            //TODO: Add logic to handle the player crossing the finish line, such as ending the game or transitioning to a new scene.
            int unlockedLevel = PlayerPrefs.GetInt("unlockedLevel", 1);
            PlayerPrefs.SetInt("unlockedLevel", unlockedLevel + 1); // Unlock the next level
            Invoke(nameof(NextLevel), reloadDelay);
        }
    }
    
    
    void NextLevel()
    {

        int unlockedLevel = PlayerPrefs.GetInt("unlockedLevel", 1);
        PlayerPrefs.SetInt("unlockedLevel", unlockedLevel + 1); // Unlock the next level
        PlayerPrefs.Save(); // Save the PlayerPrefs to persist the unlocked level

        if(unlockedLevel > 4)
        {
            // Load the main menu scene
            SceneManager.LoadScene("Menu");
        }
        else
        {
            // Load the next scene in the build index
            SceneManager.LoadScene($"Level{unlockedLevel + 1}");
        }
    }
}
