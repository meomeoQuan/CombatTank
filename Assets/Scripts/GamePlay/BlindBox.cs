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


    [Header("Tank Interaction Settings")]
    public float pushForce = 0.5f; // Lực đẩy box khi tank chạm vào
    public float pushInterval = 0.1f; // Khoảng thời gian giữa các lần đẩy liên tục
    private bool isPushingCoroutineActive = false; // Biến cờ để quản lý Coroutine đẩy


    // [Header("Drone Settings")]
    // public GameObject[] dronePrefabs; // Mảng chứa các drone prefab có thể spawn
    // public GameObject destructionEffectPrefab; // Hiệu ứng khi box vỡ (tùy chọn)

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb; // Thêm Rigidbody2D để quản lý vật lý và đẩy
    private bool isDestroyed = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        if (animator == null) Debug.LogError("Animator not found on BlindBox!");
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer not found on BlindBox!");
        if (boxCollider == null) Debug.LogError("BoxCollider2D not found on BlindBox!");
        if (rb == null) Debug.LogWarning("Rigidbody2D not found on BlindBox! Push effect might not work.");
    }

    // Hàm OnEnable() sẽ được gọi khi GameObject được kích hoạt hoặc khi được sinh ra (Instantiate)
    void OnEnable()
    {
        // 1. Reset máu về mức tối đa
        currentHealth = maxHealth;

        // 2. Reset trạng thái phá hủy
        isDestroyed = false;

        // 3. Đặt lại sprite/animation về trạng thái ban đầu
        if (spriteRenderer != null)
        {
            // Nếu bạn dùng Animator, reset trigger/boolean để về trạng thái idle
            if (animator != null)
            {
                animator.SetBool("isDestroyed", false);
                // Đảm bảo animator ở trạng thái ban đầu, hoặc reset một trigger nếu cần
            }
            // Nếu bạn dùng mảng sprite và có originalSprite
            // else if (originalSprite != null) {
            //     spriteRenderer.sprite = originalSprite;
            // }
            // Hoặc nếu destroyedSprites[0] là sprite nguyên vẹn
            // else if (destroyedSprites != null && destroyedSprites.Length > 0) {
            //     spriteRenderer.sprite = destroyedSprites[0];
            // }
            // Nếu bạn chỉ dựa vào Animator để quản lý sprite, không cần đặt ở đây
        }


        // 4. Kích hoạt lại Collider (nếu nó bị tắt khi box bị phá hủy)
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }

        // 5. Đảm bảo Rigidbody dừng mọi chuyển động (đề phòng trạng thái cũ)
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;     // Dừng chuyển động tuyến tính
            rb.angularVelocity = 0f;        // Dừng chuyển động xoay
            // Nếu Rigidbody có thể ngủ để tối ưu, có thể gọi rb.Sleep();
        }

        Debug.Log("BlindBox has been enabled/reset. Health: " + currentHealth);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // Xử lý va chạm với Tank (Player) khi nó vẫn đang trong trigger của box
        if (collision.gameObject.CompareTag("Player"))
        {
            if (rb != null && rb.bodyType != RigidbodyType2D.Static && !isPushingCoroutineActive)
            {
                StartCoroutine(ApplyContinuousPush(collision.transform.position));
            }
        }
    }

    IEnumerator ApplyContinuousPush(Vector3 tankPosition)
    {
        isPushingCoroutineActive = true;
        while (true) // Lặp vô hạn cho đến khi tank rời khỏi trigger
        {
            if (rb == null || rb.bodyType == RigidbodyType2D.Static) break; // Thoát nếu không có Rigidbody hoặc là Static

            // Tính hướng đẩy ra xa tank
            Vector2 pushDirection = (transform.position - tankPosition).normalized;
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse); // Áp dụng lực đẩy

            yield return new WaitForSeconds(pushInterval); // Chờ một chút trước khi đẩy lại
        }
        isPushingCoroutineActive = false; // Đặt lại cờ khi Coroutine kết thúc
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là đạn và box chưa bị phá hủy không
        if (!isDestroyed) return;// Chỉ xử lý va chạm nếu chưa bị phá hủy
        
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

                // Nếu bạn muốn box bị đẩy nhẹ, bạn cần Rigidbody2D trên BlindBox
                // và áp dụng lực bằng code. Logic này sẽ cần thêm OnTriggerStay2D
                // và một Coroutine như đã thảo luận trước đây nếu bạn muốn nó liên tục đẩy.
                // Ví dụ đơn giản:
                if (rb != null && rb.bodyType != RigidbodyType2D.Static)
                {
                    // Hướng đẩy từ tank ra khỏi box
                    Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
                    rb.AddForce(pushDirection * 0.5f, ForceMode2D.Impulse); // Áp dụng một lực đẩy nhẹ
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

        // Dừng mọi chuyển động vật lý ngay lập tức
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        // Dừng Coroutine đẩy liên tục nếu đang chạy
        if (isPushingCoroutineActive)
        {
            StopCoroutine("ApplyContinuousPush");
            isPushingCoroutineActive = false;
        }

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
