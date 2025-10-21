using UnityEngine;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.DataController; // để dùng DataController

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public string myBulletTag = "Shell1";
    public OwnerType owner = OwnerType.CharacterA; // chọn A hay B
    public int Damage { get; private set; }

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
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Gán Damage dựa trên owner
        SetDamage();
    }

    private void SetDamage()
    {
        if (DataController.Characters == null || DataController.Characters.Count < 2)
        {
            Damage = 0;
            return;
        }

        switch (owner)
        {
            case OwnerType.CharacterA:
                Damage = DataController.Characters[0].ATK;
                break;
            case OwnerType.CharacterB:
                Damage = DataController.Characters[1].ATK;
                break;
        }

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the bullet hit a BlindBox
        // BlindBox box = other.GetComponent<BlindBox>();
        // if (box != null)
        // {
        //     box.TakeDamage(1); // Assuming each bullet does 1 damage
        //     Destroy(gameObject); // Destroy the bullet after impact
        // }
        if (!isExploding)
        {
            Debug.Log("Bullet hit: " + other.name + $" | Damage: {Damage}");
            Explode();
        }
        // ... other collision logic for hitting bots, walls, etc.
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     // Kiểm tra xem đối tượng va chạm có phải là đạn và box chưa bị phá hủy không
    //     if (!isDestroyed) return;// Chỉ xử lý va chạm nếu chưa bị phá hủy

    //         // --- Xử lý va chạm với Đạn ---
    //         if (collision.gameObject.CompareTag("Bullet")) // Giả sử đạn có tag là "Bullet"
    //         {
    //             Bullet bullet = collision.gameObject.GetComponent<Bullet>();
    //             if (bullet != null)
    //             {
    //                 Debug.Log("BlindBox hit by bullet!");
    //                 TakeDamage(1); // Dùng damage từ bullet
    //                 // Đảm bảo script Bullet không tự phá hủy trước khi gây sát thương
    //                 Destroy(collision.gameObject); // Phá hủy viên đạn sau khi va chạm
    //             }
    //             else
    //             {
    //                 Debug.LogWarning("Bullet script not found on the collided object tagged 'Bullet'!");
    //             }
    //         }// --- Xử lý va chạm với Tank ---
    //         // Hoặc bạn có thể dùng LayerMask, GetComponent<TankController>
    //         else if (collision.gameObject.CompareTag("Player"))
    //         {
    //             Debug.Log($"BlindBox touched by a Tank ({collision.gameObject.name})!");
    //             //Ý tưởng của Thọ:

    //             // Bạn có thể quyết định:
    //             // 1. Blind Box bị phá hủy ngay lập tức khi tank chạm vào
    //             //    TakeDamage(maxHealth); // Phá hủy ngay lập tức
    //             // 2. Tank nhận sát thương/hiệu ứng khi chạm vào box
    //             //    TankController tank = collision.gameObject.GetComponent<TankController>();
    //             //    if (tank != null) tank.TakeDamage(1); // Ví dụ tank mất 1 máu

    //             // 3. Không có gì xảy ra, hoặc box bị đẩy nhẹ (nếu Rigidbody của box là Dynamic)
    //             // Hiện tại, chúng ta chỉ Debug.Log, bạn có thể thêm logic ở đây.

    //             // Nếu bạn muốn box bị đẩy nhẹ, bạn cần Rigidbody2D trên BlindBox
    //             // và áp dụng lực bằng code. Logic này sẽ cần thêm OnTriggerStay2D
    //             // và một Coroutine như đã thảo luận trước đây nếu bạn muốn nó liên tục đẩy.
    //             // Ví dụ đơn giản:
    //             if (rb != null && rb.bodyType != RigidbodyType2D.Static)
    //             {
    //                 // Hướng đẩy từ tank ra khỏi box
    //                 Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
    //                 rb.AddForce(pushDirection * 0.5f, ForceMode2D.Impulse); // Áp dụng một lực đẩy nhẹ
    //             }
    //         }

    // }


    private void Explode()
    {
        isExploding = true;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;

        if (animator != null) animator.SetBool("isExploding", true);
    }

    // Gọi ở cuối animation nổ (Animation Event)
    public void OnExplosionEnd()
    {
        Debug.Log("Animation Event: Explosion End");
        isExploding = false;
        ReturnToPool();
    }

    void OnBecameInvisible()
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        Destroy(gameObject); 
    }
}

public enum OwnerType
{
    CharacterA,
    CharacterB,
    Null
}
