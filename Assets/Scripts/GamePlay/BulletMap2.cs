using UnityEngine;

public class BulletMap2 : MonoBehaviour
{

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
        if (collision.GetComponent<EnemyMovement>()) //nếu phát hiện chuyển động của enemy
        {
            Destroy(collision.gameObject); //destroy enemy
            Destroy(gameObject); //destroy đạn
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
