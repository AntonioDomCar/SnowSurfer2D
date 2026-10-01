using UnityEngine;

public class CrashDetector : MonoBehaviour
{
   void ONTriggerEnter2D(Collider2D other)
    {
        int layerMask = LayerMask.GetMask("Floor");
        if (other.gameObject.layer == layerMask)
        {
            Debug.Log("Player has crashed!");   
    }
    }
}
