using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    [Header("Cài đặt")]
    public Transform firePoint;       // Vị trí bắn ra (thường đặt ở đầu mũi tên)
    public float bulletSpeed = 5f;    // Tốc độ bay
    public string bulletTypeTag = "Shell1";

    [Header("Phím bắn")]
    public KeyCode shootKey = KeyCode.N; // đổi thành KeyCode.Alpha3 cho Object B

    void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (BulletPoolManager.Instance == null) return;

        // Lấy đạn bằng Tag
        GameObject bullet = BulletPoolManager.Instance.GetBullet(bulletTypeTag);

        if (bullet == null) return;

        // Reset vị trí + góc
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.identity;

        float angleZ = transform.localEulerAngles.z;
        if (angleZ > 180f) angleZ -= 360f;

        float rad = (angleZ + 90f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        // Xoay sprite đạn theo hướng bay
        float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg - 90f);
    }

}
