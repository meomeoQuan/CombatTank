using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ABullet : MonoBehaviour
{
    [Header("Cài đặt đạn (mặc định)")]
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 4f;

    [Header("Offset xoay sprite (tùy chỉnh trong Inspector)")]
    public float spriteAngleOffset = -90f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float shotSpeed, int shotDamage)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        damage = shotDamage;
        float usedSpeed = (shotSpeed > 0f) ? shotSpeed : speed;        
        rb.linearVelocity = direction.normalized * usedSpeed; // Dùng velocity chứ không phải linearVelocity (đảm bảo Unity nhận đúng)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;// Xoay sprite theo hướng bắn
        transform.rotation = Quaternion.AngleAxis(angle + spriteAngleOffset, Vector3.forward);
        CancelInvoke();
        Invoke(nameof(Deactivate), lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("FortressGun")) return; // Nếu đạn chạm chính FortressGun (người bắn) thì bỏ qua
        HealthController health = other.GetComponent<HealthController>(); // Nếu đối tượng có HealthController => gây sát thương
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"💥 {other.name} nhận {damage} sát thương từ {gameObject.name}");
            Deactivate();
            return;
        }

        if (other.CompareTag("Player")) // Nếu va vào Player mà chưa có HealthController thì fallback
        {
            DualPlayerMovement player = other.GetComponent<DualPlayerMovement>();
            if (player != null)
            {
                player.SendMessage("TakeDamageFromFortress", damage, SendMessageOptions.DontRequireReceiver);
            }
            Debug.Log($"🔥 Player trúng đạn {damage} dmg!");
            Deactivate();
        }
        if (other.GetComponent<EnemyMovement>())
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
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
