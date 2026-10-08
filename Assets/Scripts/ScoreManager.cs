using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText; // Reference to the TextMeshProUGUI component for displaying the score

    public void AddScore(int score)
    {
        // Parse the current score from the text, add the new score, and update the text
        scoreText.text = score.ToString("00000");
    }

}
