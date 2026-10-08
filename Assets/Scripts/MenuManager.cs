using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   public void StartGame()
    {
        // Load the main game scene
        SceneManager.LoadScene("Level1");
    }

    public void Credits()
    {
        // Load the credits scene
        SceneManager.LoadScene("Credits");
    }

    public void Character()
    {
        // Load the character selection scene
        SceneManager.LoadScene("SelectCharacter");
    }

    public void MainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Menu");
    }

    public void GoToLevel(int level)
    {
        SceneManager.LoadScene($"Level{level}"); // Load the specified level scene
    }

    public void SelectLevel()
    {
        // Load the level selection scene
        SceneManager.LoadScene("SelectLevel");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }

    public void SelectCharacter(int Character)
    {
        PlayerPrefs.SetInt("SelectedCharacter", Character); // Save the selected character index in PlayerPrefs
        PlayerPrefs.Save();

        SceneManager.LoadScene("Menu"); // Load the main menu scene after selecting a character
    }
}
