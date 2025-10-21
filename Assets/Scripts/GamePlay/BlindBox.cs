using System.Collections;
using UnityEngine;

public class BlindBox : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Feedback Effects")]
    // public GameObject destructionEffectPrefab;
    public float finalDisplayDuration = 0.5f;

    [Header("Tank Interaction Settings")]
    public float pushSpeed = 1.5f; // Tốc độ box bị đẩy khi tank chạm vào

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private bool isDestroyed = false;

    // Cờ để kiểm tra xem box có đang bị đẩy trong frame hiện tại kh
    private bool isBeingPushedThisFrame = false;

    private OwnerType lastAttacker;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null) Debug.LogWarning("Rigidbody2D not found on BlindBox! Push effect will not work.");
        if (boxCollider == null) Debug.LogWarning("BoxCollider2D not found on BlindBox! Collision detection will not work.");
        if (animator == null) Debug.LogWarning("Animator not found on BlindBox! Destruction animation will not work.");
        if (spriteRenderer == null) Debug.LogWarning("SpriteRenderer not found on BlindBox! Damage feedback will not work.");
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        isDestroyed = false;
        lastAttacker = OwnerType.Null;

        if (animator != null) animator.SetBool("isDestroyed", false);
        if (boxCollider != null) boxCollider.enabled = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // Sử dụng FixedUpdate để quản lý vật lý một cách đáng tin cậy
    void FixedUpdate()
    {
        if (isDestroyed) return;

        // Nếu trong frame vật lý này, box không bị đẩy,set vận tốc của nó về 0
        if (!isBeingPushedThisFrame && rb.linearVelocity != Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Reset cờ ở cuối mỗi frame vật lý
        isBeingPushedThisFrame = false;
    }


    #region Physics Interactions

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDestroyed || !other.CompareTag("Player")) return;

        if (rb != null && rb.bodyType != RigidbodyType2D.Static)
        {
            // 1. Tính toán hướng đẩy ra xa khỏi tank
            Vector2 pushDirection = (transform.position - other.transform.position).normalized;
            //normalized Chuẩn hóa vector: biến nó thành vector đơn vị (độ dài = 1), chỉ giữ lại hướng.
            //Giúp bạn dùng hướng này để đẩy, di chuyển, hoặc áp lực mà không bị ảnh hưởng bởi khoảng cách.
            
            // 2. Thiết lập vận tốc của box trực tiếp theo hướng đó
            //  giúp box di chuyển mượt mà và dừng lại ngay khi tank dừng
            rb.linearVelocity = pushDirection * pushSpeed;

            // 3. Đặt cờ để FixedUpdate biết rằng box đang được đẩy
            isBeingPushedThisFrame = true;
        }
    }

    // REMOVED: Toàn bộ Coroutine và OnTriggerExit2D cho việc đẩy không còn cần thiết nữa
    // Logic mới trong FixedUpdate và OnTriggerStay2D đã xử lý tất cả các trường hợp
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the bullet hit a BlindBox
        // BlindBox box = other.GetComponent<BlindBox>();
        // if (box != null)
        // {
        //     box.TakeDamage(1); // Assuming each bullet does 1 damage
        //     Destroy(gameObject); // Destroy the bullet after impact
        // }

        if (isDestroyed) return;

        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                lastAttacker = bullet.owner;
                TakeDamage(1);
                Destroy(other.gameObject);
            }
        }
        // ... other collision logic for hitting bots, walls, etc.
    }
    #endregion


    #region Damage and Destruction

    public void TakeDamage(int damageAmount)
    {
        if (isDestroyed) return;

        currentHealth -= damageAmount;
        Debug.Log($"Blind Box Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            DestroyBox();
        }
        else
        {
            StartCoroutine(FlashDamageEffect());  // hàm kiểu IEnumerator để chạy bất đồng bộ (không chặn luồng chính)
        }
    }

    void DestroyBox()
    {
        isDestroyed = true;

        if (boxCollider != null) boxCollider.enabled = false;
        // Đảm bảo box dừng hoàn toàn khi bị phá hủy
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("isDestroyed", true);
        
        // string attackerName = (lastAttacker == OwnerType.CharacterA) ? "Player A" : "Player B";
        // Debug.Log($"Box bị phá bởi {attackerName}!");

        // if (destructionEffectPrefab != null)
        // {
        //     Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        // }


        //  Logic trao phần thưởng ---
        if (lastAttacker != OwnerType.Null)
        {
            DualPlayerMovement attackerPlayer = GameManager.Instance.GetPlayerByType(lastAttacker);
            if (attackerPlayer != null)
            {
                // 3. Lấy PowerUpManager từ GameObject của người chơi đó
                PowerUpManager powerUpManager = attackerPlayer.GetComponent<PowerUpManager>();
                if (powerUpManager != null)
                {
                    // 4. Trao phần thưởng
                    powerUpManager.GrantRandomReward();
                    Debug.Log($"Phần thưởng được trao cho {attackerPlayer.name}");
                }
                else
                {
                    Debug.LogWarning($"Người chơi {attackerPlayer.name} không có PowerUpManager!");
                }
            }
        }
        else
        {
            Debug.LogWarning("Không xác định được người phá hủy Blind Box, không có phần thưởng nào được trao.");
        }

        // PowerUpManager powerUpManager = FindObjectOfType<PowerUpManager>();
        // if (powerUpManager != null)
        // {
        //     powerUpManager.GrantRandomReward();
        // }
        // else
        // {
        //     Debug.LogError("CRITICAL: PowerUpManager was not found in the scene! No reward can be granted.");
        // }

        StartCoroutine(CleanupAfterAnimation());
    }

    IEnumerator CleanupAfterAnimation()
    {
        yield return new WaitForSeconds(finalDisplayDuration);
        Destroy(gameObject);
    }

    IEnumerator FlashDamageEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    #endregion
}