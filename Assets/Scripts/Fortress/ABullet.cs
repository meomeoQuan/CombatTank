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

    // Gọi từ FortressGun ngay sau khi Instantiate
    public void Launch(Vector2 direction, float shotSpeed, int shotDamage)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        damage = shotDamage;
        float usedSpeed = (shotSpeed > 0f) ? shotSpeed : speed;

        rb.linearVelocity = direction.normalized * usedSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + spriteAngleOffset, Vector3.forward);

        CancelInvoke();
        Invoke(nameof(Deactivate), lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra trúng nhân vật (tank)
        var player = other.GetComponent<DualPlayerMovement>();
        if (player != null)
        {
            // 💥 Gây damage lên tank
            player.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Deactivate();
            return;
        }

        // Nếu đụng tường hoặc vật cản thì hủy luôn
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
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
