using UnityEngine;

public class Portal : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("<color=blue>[Portal]</color> Player entered the portal. Loading next level...");
            SenceControllerMap2.instance.LoadScene("BossSence");
        }
    }
}
