//using UnityEngine;
//using System.Collections;

//public class SoldierAutoMove : MonoBehaviour
//{
//    [SerializeField] private GameObject Tank;
//    [SerializeField] private float speed = 1.5f;


//    void Start()
//    {

//    }

//    private void Update()
//    {
//        transform.position = Vector2.MoveTowards(transform.position, Tank.transform.position, speed * Time.deltaTime);
//        transform.up = Tank.transform.position - transform.position;

//    }

//}

using UnityEngine;

public class SoldierAutoMove : MonoBehaviour
{
    [SerializeField] private GameObject Tank;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float avoidForce = 1f;
    [SerializeField] private float detectionDistance = 1f;
    [SerializeField] private LayerMask obstacleMask;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Tank == null) return;

        Vector2 direction = (Tank.transform.position - transform.position).normalized;
        Vector2 avoidance = Vector2.zero;

        // Bắn tia raycast về phía trước
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, obstacleMask);

        if (hit.collider != null)
        {
            // Né sang trái hoặc phải (vuông góc với vật cản)
            Vector2 hitNormal = hit.normal;
            avoidance = Vector2.Perpendicular(hitNormal) * avoidForce;
        }

        // Hướng tổng hợp: hướng đến tank + né vật cản
        Vector2 finalDir = (direction + avoidance).normalized;

        // Di chuyển
        rb.MovePosition(rb.position + finalDir * speed * Time.fixedDeltaTime);

        // Xoay mặt về hướng di chuyển
        transform.up = finalDir;
        Debug.DrawRay(transform.position, direction * detectionDistance, Color.red);

    }

    private void OnDrawGizmosSelected()
    {
        if (Tank == null) return;
        Gizmos.color = Color.red;
        Vector2 direction = (Tank.transform.position - transform.position).normalized;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * detectionDistance);
    }
}
