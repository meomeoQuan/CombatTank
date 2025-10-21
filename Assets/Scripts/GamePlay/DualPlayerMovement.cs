using UnityEngine;
using Assets.Scripts.DataController;
using Assets.Scripts.Models.Characters;
using UnityEngine.UI;

public class DualPlayerMovement : MonoBehaviour
{
    public enum CharacterType { CharacterA, CharacterB }
    public CharacterType characterType;

    private Character characterData;
    private Rigidbody2D rb;
    private Vector2 movement;
    [Header("UI")]
    public Image healthImage; // drag Image máu trong Canvas vào đây
    public Transform spawnPoint; // Kéo vào từ GameManager hoặc gán sẵn trong Inspector

    // HP hiện tại (có thể bị trừ khi trúng đạn)
    public int currentHP { get; private set; }
    public int maxHP { get; private set; }

    // Ranh giới di chuyển
    public BoxCollider2D boundaryCollider;
    private Bounds boundary;

    [Header("Movement Settings")]
    [SerializeField] private float turnSmoothing = 10f; // Mượt xoay hướng tank
    private float lastMoveMagnitude = 0f;
    //========================================== SETUP DỮ LIỆU VÀ DI CHUYỂN CHO NHÂN VẬT ========================================
    void Start()
    {
        // Tho
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterPlayer(this);
        else
            Debug.LogError("FATAL: GameManager not found in the scene!");
        //
        rb = GetComponent<Rigidbody2D>();

        if (DataController.Characters.Count >= 2)
        {
            characterData = (characterType == CharacterType.CharacterA)
                ? DataController.Characters[0]
                : DataController.Characters[1];
        }

        if (characterData != null)
        {
            maxHP = characterData.HP; // gán máu ban đầu
            currentHP = maxHP;
            Debug.Log($"{characterType} Start HP = {currentHP}");
        }

        if (boundaryCollider != null)
        {
            boundary = boundaryCollider.bounds;
        }
    }
    //========================================== DI CHUYỂN NHÂN VẬT ========================================
    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsGameRunning)

            return; // Chặn mọi input khi game chưa chạy hoặc đã kết thúc

        if (characterType == CharacterType.CharacterA)
        {
            movement.x = Input.GetAxisRaw("Horizontal_WASD");
            movement.y = Input.GetAxisRaw("Vertical_WASD");
        }
        else if (characterType == CharacterType.CharacterB)
        {
            movement.x = Input.GetAxisRaw("Horizontal_Arrows");
            movement.y = Input.GetAxisRaw("Vertical_Arrows");
        }

        // if (movement != Vector2.zero)
        // {
        //     float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        //     transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        // }
        // Mượt xoay hướng tank khi di chuyển
        if (movement.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * turnSmoothing);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsGameRunning)
            return; // Chặn di chuyển luôn

        Vector2 normalizedMovement = movement.normalized;
        float moveSpeed = (characterData != null) ? characterData.Speed : 2f;

        Vector2 newPosition = rb.position + normalizedMovement * moveSpeed * Time.fixedDeltaTime;

        if (boundaryCollider != null)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, boundary.min.x, boundary.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, boundary.min.y, boundary.max.y);
        }

        rb.MovePosition(newPosition);
    }


    //========================================== XỬ LÝ ĐẠN BẮN TRÚNG ========================================
    void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            // Đạn của phe A chỉ trúng B và ngược lại
            if ((characterType == CharacterType.CharacterA && bullet.owner == OwnerType.CharacterB) ||
                (characterType == CharacterType.CharacterB && bullet.owner == OwnerType.CharacterA))
            {
                TakeDamage(bullet.Damage);
            }

        }
    }

    private void TakeDamage(int damage)
    {
        int armor = (characterData != null) ? characterData.Armor : 0;
        int finalDamage = Mathf.Max(1, damage - armor); // đảm bảo ít nhất gây 1 damage

        currentHP -= finalDamage;
        if (currentHP < 0) currentHP = 0;
        UpdateHealthUI();
        Debug.Log($"{characterType} bị trúng đạn! Dame: {damage}, Armor: {armor}, Mất {finalDamage} HP, còn {currentHP} HP");

        if (currentHP == 0)
        {
            Die();
        }
    }


    private void Die()
    {
        Debug.Log($"{characterType} đã chết!");
        GameManager.Instance.EndGame(characterType);
        // TODO: Thêm hiệu ứng nổ, disable nhân vật hoặc reload game

    }
    // Tho
    public void Heal(int healAmount)
    {
        // Chỉ hồi máu nếu nhân vật chưa chết và chưa đầy máu
        if (currentHP <= 0 || currentHP == maxHP)
        {
            Debug.Log($"{characterType} không thể hồi máu (đã chết hoặc đã đầy máu).");
            return;
        }

        currentHP += healAmount;
        // Đảm bảo máu không vượt quá mức tối đa
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        UpdateHealthUI(); // Cập nhật lại thanh máu trên UI
        Debug.Log($"{characterType} được hồi {healAmount} HP, máu hiện tại: {currentHP}/{maxHP}");
    }

    public void ResetCharacter()
    {
        // Reset máu
        currentHP = maxHP;
        UpdateHealthUI();

        // Reset vị trí
        if (spawnPoint != null)
        {
            rb.position = spawnPoint.position;
            transform.position = spawnPoint.position;
            transform.rotation = Quaternion.identity;
        }
    }

    void UpdateHealthUI()
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = (float)currentHP / maxHP;
        }
    }
}
