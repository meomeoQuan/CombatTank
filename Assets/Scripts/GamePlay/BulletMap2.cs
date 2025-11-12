using UnityEngine;

public class BulletMap2 : MonoBehaviour
{
    [Header("Cài đặt đạn")]
    public float damage = 10f; // 💥 sát thương mỗi viên
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        DestroyWhenOffScreen();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 💥 Nếu va vào enemy thường
        if (collision.GetComponent<EnemyMovement>())
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        // 💥 Nếu va vào FortressGun
        else if (collision.GetComponent<FortressGun>())
        {
            // tìm HealthController
            var health = collision.GetComponent<HealthController>();
            if (health != null)
            {
                health.TakeDamage(damage); // ✅ gây sát thương
            }

            // 💥 huỷ đạn
            Destroy(gameObject);
        }
    }

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        // ❌ Huỷ khi bay ra ngoài màn hình
        if (screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }
}
