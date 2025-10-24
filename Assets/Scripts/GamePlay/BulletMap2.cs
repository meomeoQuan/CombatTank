using UnityEngine;

public class BulletMap2 : MonoBehaviour
{
        [SerializeField] private float _damageAmount = 25f; 
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        DestroyWhenOffScreen();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if object has HealthController
        HealthController health = collision.GetComponent<HealthController>();
        if (health != null)
        {
            health.TakeDamage(_damageAmount);
            Debug.Log($"<color=yellow>[Bullet]</color> Hit {collision.gameObject.name}, dealing {_damageAmount} damage!");

            // Destroy bullet after hitting
            Destroy(gameObject);
            return;
        }

       
    }

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position); //xác định vị trí đạn 

        if (screenPosition.x < 0 ||
           screenPosition.x > _camera.pixelWidth ||
           screenPosition.y < 0 ||
           screenPosition.y > _camera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }
}
