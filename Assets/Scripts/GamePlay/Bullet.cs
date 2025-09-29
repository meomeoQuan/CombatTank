using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string myBulletTag = "Shell1";
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isExploding = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        // Reset trạng thái mỗi lần spawn
        isExploding = false;
        if (animator != null) animator.SetBool("isExploding", false);
        if (col != null) col.enabled = true;
        if (rb != null) rb.linearVelocity = Vector2.zero; // bạn có thể set lại trong chỗ bắn ra
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isExploding)
        {
            Debug.Log("Bullet hit: " + collision.name);
            Explode();
        }
    }

    private void Explode()
    {
        isExploding = true;

        // Ngừng di chuyển & tắt collider
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;

        // Bật animation nổ
        animator.SetBool("isExploding", true);
    }

    // Hàm này gọi ở cuối animation nổ (Animation Event)
    public void OnExplosionEnd()
    {
        isExploding = false;
        ReturnToPool();
    }
    void OnBecameInvisible()
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (BulletPoolManager.Instance != null && !string.IsNullOrEmpty(myBulletTag))
        {
            BulletPoolManager.Instance.ReturnBullet(gameObject, myBulletTag);
        }
    }
}
