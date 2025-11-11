using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ABullet : MonoBehaviour
{
    [Header("Cài đặt đạn")]
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 4f;
    public float spriteAngleOffset = -90f;

    private Rigidbody2D rb;
    public GameObject shooter; // Ai bắn ra viên đạn

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float shotSpeed, GameObject shooterObj = null)
    {
        shooter = shooterObj; // Lưu lại người bắn
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        float usedSpeed = (shotSpeed > 0f) ? shotSpeed : speed;
        rb.linearVelocity = direction.normalized * usedSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);

        CancelInvoke();
        Invoke(nameof(Deactivate), lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Bỏ qua chính đạn cùng loại
        if (other.CompareTag("Bullet")) return;

        // Nếu đạn bắn từ FortressGun va vào FortressGun thì bỏ qua
        if (shooter != null && shooter.CompareTag("FortressGun") && other.CompareTag("FortressGun"))
        {
            return;
        }

        // Va vào Player
        if (other.CompareTag("Player"))
        {
            HealthController hc = other.GetComponent<HealthController>();
            if (hc != null)
            {
                hc.TakeDamage(damage);
                Debug.Log($"🔥 Player {other.name} nhận {damage} dmg từ {shooter?.name}, máu còn: {hc.CurrentHealth}");
            }
            Deactivate();
            return;
        }

        // Va vào Enemy
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"💥 Enemy {other.name} bị tiêu diệt bởi {shooter?.name}");
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        // Va vào đối tượng có HealthController (khác Player/Enemy)
        HealthController health = other.GetComponent<HealthController>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"💥 {other.name} nhận {damage} dmg từ {shooter?.name}, máu còn: {health.CurrentHealth}");
            Deactivate();
        }
    }


    void Deactivate()
    {
        CancelInvoke();
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Deactivate();
    }
}
