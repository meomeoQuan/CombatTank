using UnityEngine;
using Assets.Scripts.DataController;
using Assets.Scripts.Models.Characters;
using UnityEngine.UI;

public class SinglePlayerMovement : MonoBehaviour
{
    private Character characterData;
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("UI")]
    public Image healthImage; // Drag the health Image from Canvas
    public Transform spawnPoint; // Assign from GameManager or Inspector

    [Header("Health Settings")]
    public int currentHP { get; private set; }
    public int maxHP { get; private set; }

    [Header("Boundary")]
    public BoxCollider2D boundaryCollider;
    private Bounds boundary;

    [Header("Movement Settings")]
    [SerializeField] private float turnSmoothing = 10f; // Smooth rotation
    private float lastMoveMagnitude = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (characterData != null)
        {
            maxHP = characterData.HP;
            currentHP = maxHP;
            Debug.Log($"Start HP = {currentHP}");
        }
        else
        {
            // Fallback default HP
            maxHP = 100;
            currentHP = maxHP;
            Debug.LogWarning("Character data not assigned! Using default HP = 100");
        }

        if (boundaryCollider != null)
        {
            boundary = boundaryCollider.bounds;
        }
    }

    private void Update()
    {
        // Fallback for input axis (in case not configured)
        float moveX = Input.GetAxisRaw("Horizontal_WASD");
        float moveY = Input.GetAxisRaw("Vertical_WASD");

        

        movement = new Vector2(moveX, moveY);

        // Rotate smoothly toward movement direction
        if (movement.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * turnSmoothing);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void FixedUpdate()
    {
        Vector2 normalizedMovement = movement.normalized;
        float moveSpeed = (characterData != null) ? characterData.Speed : 2f;

        Vector2 newPosition = rb.position + normalizedMovement * moveSpeed * Time.fixedDeltaTime;

        // Clamp position inside boundary
        if (boundaryCollider != null)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, boundary.min.x, boundary.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, boundary.min.y, boundary.max.y);
        }

        rb.MovePosition(newPosition);
    }

    public void ResetCharacter()
    {
        // Reset HP
        currentHP = maxHP;
        UpdateHealthUI();

        // Reset position and rotation
        if (spawnPoint != null)
        {
            rb.position = spawnPoint.position;
            transform.position = spawnPoint.position;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Spawn point not assigned!");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = (float)currentHP / maxHP;
        }
    }

    // Optional: Assign character data at runtime
    public void SetCharacterData(Character data)
    {
        characterData = data;
        maxHP = data.HP;
        currentHP = maxHP;
        UpdateHealthUI();
    }
}
