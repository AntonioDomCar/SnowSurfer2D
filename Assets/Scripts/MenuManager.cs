using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   public void StartGame()
    {
        // Load the main game scene
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
}
