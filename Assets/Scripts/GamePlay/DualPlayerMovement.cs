using UnityEngine;

public class DualPlayerMovement : MonoBehaviour
{
    public enum PlayerType { PlayerA, PlayerB }
    public PlayerType player;

    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 movement;

    // Thêm biến để xác định ranh giới di chuyển
    public BoxCollider2D boundaryCollider;
    private Bounds boundary;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Lấy ranh giới từ collider
        if (boundaryCollider != null)
        {
            boundary = boundaryCollider.bounds;
        }
    }

    void Update()
    {
        if (player == PlayerType.PlayerA)
        {
            // Sử dụng trục WASD để di chuyển PlayerA
            movement.x = Input.GetAxisRaw("Horizontal_WASD");
            movement.y = Input.GetAxisRaw("Vertical_WASD");
        }
        else if (player == PlayerType.PlayerB)
        {
            // Sử dụng trục phím mũi tên để di chuyển PlayerB
            movement.x = Input.GetAxisRaw("Horizontal_Arrows");
            movement.y = Input.GetAxisRaw("Vertical_Arrows");
        }

        // ---- Xử lý rotation ----
        if (movement != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;

            if (player == PlayerType.PlayerA)
            {
                // Player 1 quay đúng hướng
                transform.rotation = Quaternion.Euler(0, 0, angle-90);
            }
            else if (player == PlayerType.PlayerB)
            {
                // Player 2 quay ngược lại
                transform.rotation = Quaternion.Euler(0, 0, angle-90);
            }
        }
    }


    void FixedUpdate()
    {
        Vector2 normalizedMovement = movement.normalized;
        Vector2 newPosition = rb.position + normalizedMovement * moveSpeed * Time.fixedDeltaTime;

        // Giới hạn vị trí mới trong ranh giới đã định
        if (boundaryCollider != null)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, boundary.min.x, boundary.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, boundary.min.y, boundary.max.y);
        }

        rb.MovePosition(newPosition);
    }
}