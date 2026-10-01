using UnityEngine;

public class FinishLine : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has crossed the finish line!");
            //TODO: Add logic to handle the player crossing the finish line, such as ending the game or transitioning to a new scene.
        }
    }
}
