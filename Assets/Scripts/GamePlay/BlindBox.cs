using UnityEngine;

public class BlindBox : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3; // Máu tối đa của Blind Box
    private int currentHealth; // Máu hiện tại

    [Header("Sprite Settings")]
    public Sprite[] destroyedSprites; // Mảng chứa các sprite animation khi bị phá hủy
    public float destructionDelay = 0.1f; // Thời gian giữa các frame animation

    [Header("Drone Settings")]
    public GameObject[] dronePrefabs; // Mảng chứa các drone prefab có thể spawn
    public GameObject destructionEffectPrefab; // Hiệu ứng khi box vỡ (tùy chọn)

    private int currentSpriteIndex = 0;
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

        // Đảm bảo sprite ban đầu là sprite nguyên vẹn
        if (destroyedSprites.Length > 0 && spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = destroyedSprites[0]; // Giả định sprite đầu tiên là nguyên vẹn
        }

        if (spriteRenderer == null) Debug.LogError("SpriteRenderer not found on BlindBox!");
        if (boxCollider == null) Debug.LogError("BoxCollider2D not found on BlindBox!");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Giả sử đạn của bạn có tag là "Bullet"
        if (collision.gameObject.CompareTag("Bullet") && !isDestroyed)
        {
            Debug.Log("BlindBox hit by bullet!");
            DestroyBox();
            // Destroy(collision.gameObject); // Phá hủy viên đạn sau khi va chạm
        }
    }

    void DestroyBox()
    {
        isDestroyed = true;
        // Tắt collider để đạn không va chạm nữa
         if (boxCollider != null) boxCollider.enabled = false;

        // if (animator != null) animator.SetBool("isDestroyed", true);


        // Kích hoạt hiệu ứng phá hủy (nếu có)
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Bắt đầu animation phá hủy
        StartCoroutine(AnimateDestruction());

        // Spawn ngẫu nhiên một drone
        // SpawnRandomDrone();
    }

    System.Collections.IEnumerator AnimateDestruction()
    {
        for (int i = 0; i < destroyedSprites.Length; i++)
        {
            spriteRenderer.sprite = destroyedSprites[i];
            yield return new WaitForSeconds(destructionDelay);
        }

        // Khi animation kết thúc, phá hủy blind box
        Destroy(gameObject);
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
