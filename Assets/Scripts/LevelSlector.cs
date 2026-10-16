using UnityEngine;
using UnityEngine.UI;

public class LevelSlector : MonoBehaviour
{
    public Color unlockedColor = Color.green;
    public Color lockedColor = Color.gray;

    void Start()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        Transform levels = transform.GetChild(0);

        for (int i = 0; i < levels.childCount; i++)
        {
            Button levelButton = levels.GetChild(i).GetComponent<Button>();

            if (levelButton == null)
                continue;

            // Los botones empiezan en el índice 0,
            // pero los niveles empiezan en el 1
            bool isUnlocked = (i + 1) <= unlockedLevels;

            levelButton.interactable = isUnlocked;

            Image buttonImage = levelButton.GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImage.color = isUnlocked
                    ? unlockedColor
                    : lockedColor;
            }
        }
    }
}