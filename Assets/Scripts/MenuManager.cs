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
    int unlockedLevels = Mathf.Max(
        1,
        PlayerPrefs.GetInt("UnlockedLevels", 1)
    );

    Debug.Log("Intentando entrar al nivel: " + level);
    Debug.Log("Niveles desbloqueados: " + unlockedLevels);

    if (level > unlockedLevels || level < 1)
    {
        Debug.LogWarning("Nivel bloqueado: " + level);
        return;
    }

    SceneManager.LoadScene("Level" + level);
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

    public void SelectCharacter(int character)
    {
        PlayerPrefs.SetInt("SelectedCharacter", character);
        PlayerPrefs.Save();

        Debug.Log("Personaje guardado: " +
            PlayerPrefs.GetInt("SelectedCharacter"));

        SceneManager.LoadScene("Menu");
    }
}
