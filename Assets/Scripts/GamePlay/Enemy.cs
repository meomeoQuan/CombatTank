using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;
    [SerializeField] float speed = 3f;

    [Header("Scale Settings")]
    [SerializeField] Vector3 scaleFacingRight = new Vector3(3, 3, 1);
    [SerializeField] Vector3 scaleFacingLeft = new Vector3(-3, 3, 1);

    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get direction to player
        Vector3 direction = (player.position - transform.position).normalized;
        moveDirection = direction;

        Debug.Log($"Enemy Position: {transform.position}, Player Position: {player.position}, Direction: {direction}");
        // Flip enemy based on player's position
        if (player.position.x > transform.position.x)
        {
            transform.localScale = scaleFacingRight;
        }
        else
        {
            transform.localScale = scaleFacingLeft;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }
}
