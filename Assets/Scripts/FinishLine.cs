using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float reloadDelay = 1f;
    [SerializeField] private ParticleSystem finishEffect;

    private bool finished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Player") || finished)
            return;

        finished = true;

        Debug.Log("Player has crossed the finish line!");

        if (finishEffect != null)
            finishEffect.Play();
        
        
        // Get the current level number from the scene name
        string sceneName = SceneManager.GetActiveScene().name;
        string levelText = sceneName.Replace("Level", "");

        if (!int.TryParse(levelText, out int currentLevel))
        {
            Debug.LogError("Scene name must follow the format Level1, Level2, etc.");
            return;
        }

        // Unlock the next level
        int nextLevel = currentLevel + 1;
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevels", 1);

        if (nextLevel > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevels", nextLevel);
            PlayerPrefs.Save();
        }

        Invoke(nameof(NextLevel), reloadDelay);
    }

    private void NextLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string levelText = sceneName.Replace("Level", "");

        if (!int.TryParse(levelText, out int currentLevel))
            return;

        int nextLevel = currentLevel + 1;

        // Return to the menu after the final level
        if (currentLevel >= 4)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            SceneManager.LoadScene("Level" + nextLevel);
        }
    }
}