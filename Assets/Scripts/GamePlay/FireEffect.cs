using UnityEngine;

public class FireEffect : MonoBehaviour
{
    [SerializeField] private float _duration = 3f;

    private void Start()
    {
        Destroy(gameObject, _duration);
    }

    // optional: damage player when standing on it
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // damage logic here
            Debug.Log("🔥 Player is burning!");
            Destroy(gameObject);
        }
    }
}
