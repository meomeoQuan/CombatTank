using System.Collections;
using UnityEngine;

public class BlindBox : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3; // Máu tối đa của Blind Box
    private int currentHealth; // Máu hiện tại

    [Header("Sprite Settings")]
    public float destructionDelay = 0.2f; // Thời gian giữa các frame animation
    public float finalDisplayDuration = 5f; // Thời gian hiển thị sprite cuối cùng (0.5 giây)
    public GameObject destructionEffectPrefab; // Hiệu ứng khi box vỡ (tùy chọn)

    // [Header("Drone Settings")]
    // public GameObject[] dronePrefabs; // Mảng chứa các drone prefab có thể spawn
    // public GameObject destructionEffectPrefab; // Hiệu ứng khi box vỡ (tùy chọn)

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private bool isDestroyed = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        currentHealth = maxHealth;

        if (animator == null) Debug.LogError("Animator not found on BlindBox!");
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer not found on BlindBox!");
        if (boxCollider == null) Debug.LogError("BoxCollider2D not found on BlindBox!");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là đạn và box chưa bị phá hủy không
        if (!isDestroyed) // Chỉ xử lý va chạm nếu chưa bị phá hủy
        {
            // --- Xử lý va chạm với Đạn ---
            if (collision.gameObject.CompareTag("Bullet")) // Giả sử đạn có tag là "Bullet"
            {
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (bullet != null)
                {
                    Debug.Log("BlindBox hit by bullet!");
                    TakeDamage(1); // Dùng damage từ bullet
                    // Đảm bảo script Bullet không tự phá hủy trước khi gây sát thương
                    Destroy(collision.gameObject); // Phá hủy viên đạn sau khi va chạm
                }
                else
                {
                    Debug.LogWarning("Bullet script not found on the collided object tagged 'Bullet'!");
                }
            }// --- Xử lý va chạm với Tank ---
            // Hoặc bạn có thể dùng LayerMask, GetComponent<TankController>
            else if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log($"BlindBox touched by a Tank ({collision.gameObject.name})!");
                //Ý tưởng của Thọ:

                // Bạn có thể quyết định:
                // 1. Blind Box bị phá hủy ngay lập tức khi tank chạm vào
                //    TakeDamage(maxHealth); // Phá hủy ngay lập tức
                // 2. Tank nhận sát thương/hiệu ứng khi chạm vào box
                //    TankController tank = collision.gameObject.GetComponent<TankController>();
                //    if (tank != null) tank.TakeDamage(1); // Ví dụ tank mất 1 máu

                // 3. Không có gì xảy ra, hoặc box bị đẩy nhẹ (nếu Rigidbody của box là Dynamic)
                // Hiện tại, chúng ta chỉ Debug.Log, bạn có thể thêm logic ở đây.
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDestroyed) return; // Không nhận sát thương nếu đã bị phá hủy

        currentHealth -= damageAmount;
        Debug.Log($"Blind Box took {damageAmount} damage. Current Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            DestroyBox(); // Nếu máu hết, phá hủy box
        }
        else
        {
            // (Tùy chọn) Thêm hiệu ứng visual hoặc âm thanh khi bị bắn nhưng chưa vỡ
            // Ví dụ: Nhấp nháy màu, phát âm thanh "bị trúng"
            StartCoroutine(FlashDamageEffect());
        }
    }

    // (Tùy chọn) Hiệu ứng nhấp nháy khi bị bắn
    IEnumerator FlashDamageEffect()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red; // Hoặc một màu khác
        yield return new WaitForSeconds(destructionDelay); // Thời gian nhấp nháy
        spriteRenderer.color = originalColor;
    }

    void DestroyBox()
    {
        isDestroyed = true;
        // Tắt collider để đạn không va chạm nữa
        if (boxCollider != null) boxCollider.enabled = false;

        // Bắt đầu animation phá hủy
        if (animator != null) animator.SetBool("isDestroyed", true);
        else Debug.LogWarning("Animator is null, cannot play destruction animation.");

        // Kích hoạt hiệu ứng phá hủy (nếu có)
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Spawn ngẫu nhiên một drone
        // SpawnRandomDrone();

        // Đặt một Coroutine để chờ animation hoàn tất và thêm delay cuối cùng
        // Destroy(gameObject, finalDisplayDuration); // Chờ 5 giây để animation hoàn thành trước khi phá hủy object
        StartCoroutine(WaitForAnimationAndDestroy());
    }

    IEnumerator WaitForAnimationAndDestroy()
    {
        yield return new WaitForSeconds(finalDisplayDuration); // Đặt thời gian delay này sao cho bao gồm cả thời gian animation vỡ và thời gian hiển thị cuối cùng.
        Debug.Log("Blind Box animation finished and destroyed after delay.");
        Destroy(gameObject); // Phá hủy GameObject sau khi đợi
    }
    // void SpawnRandomDrone()
        // {
        //     if (dronePrefabs.Length == 0)
        //     {
        //         Debug.LogWarning("No drone prefabs assigned to BlindBox!");
        //         return;
        //     }

        //     int randomIndex = Random.Range(0, dronePrefabs.Length);
        //     GameObject selectedDronePrefab = dronePrefabs[randomIndex];

        //     // Vị trí spawn drone: có thể là ngay tại vị trí blind box, hoặc hơi nhích lên
        //     Vector3 spawnPosition = transform.position;
        //     // Nếu muốn drone bay ra từ trên cao:
        //     // spawnPosition.z = -1; // Đặt Z thấp hơn nếu drone có z-value khác

        //     Instantiate(selectedDronePrefab, spawnPosition, Quaternion.identity);
        //     Debug.Log("Spawned drone: " + selectedDronePrefab.name);
        // }
    }
