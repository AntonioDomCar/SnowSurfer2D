using UnityEngine;
using UnityEngine.UI;

public class LevelSlector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the number of unlocked levels from PlayerPrefs, default is 1
        int unlockedlevels = PlayerPrefs.GetInt("UnlockedLevels", 1); 

        // Get the first child of the current transform, which contains the level buttons
        Transform levels = transform.GetChild(0); 

        // Loop through each level button and set its interactable state based on unlocked levels
        for (int i = 0; i < levels.childCount; i++)
        {
            //Enable the botton if the level is unlocked, otherwise disable it
           Button levelButton = levels.GetChild(i).GetComponent<Button>();
           levelButton.interactable = (i < unlockedlevels);
        }
    }

    
}
