//using UnityEngine;

//public class BulletMap2 : MonoBehaviour
//{

//    private Camera _camera;
//    [SerializeField] private float _damage = 10f;
//    private void Awake()
//    {
//        _camera = Camera.main;
//    }

//    private void Update()
//    {
//        DestroyWhenOffScreen();


//        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1f, LayerMask.GetMask("Enemy"));
//        if (hit.collider != null)
//        {
//            Debug.Log($"[Bullet] Raycast hit: {hit.collider.name}");
//            var health = hit.collider.GetComponent<HealthController>();
//            if (health != null)
//            {
//                Debug.Log($"[Bullet] Raycast hit {hit.collider.name}");
//                health.TakeDamage(_damage);
//                Destroy(gameObject);
//            }
//        }
//        else
//        {
//            Debug.Log("[Bullet] Raycast không thấy collider nào");
//        }
//        //Collider2D hit = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Enemy"));
//        //if (hit != null)
//        //{
//        //    var health = hit.GetComponent<HealthController>();
//        //    if (health != null)
//        //    {
//        //        health.TakeDamage(_damage);
//        //        Destroy(gameObject);
//        //    }
//        //}


//    }


//    private void DestroyWhenOffScreen()
//    {
//        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position); //xác định vị trí đạn 

//        if (screenPosition.x < 0 ||
//           screenPosition.x > _camera.pixelWidth ||
//           screenPosition.y < 0 ||
//           screenPosition.y > _camera.pixelHeight)
//        {
//            Destroy(gameObject);
//        }
//    }
//}

////// Start is called once before the first execution of Update after the MonoBehaviour is created
//////private void OnTriggerEnter2D(Collider2D collision)
//////{
//////    if (collision.GetComponent<EnemyMovement>()) //Kiểm tra xem đối tượng chạm vào có script EnemyMovement đối tượng là enemy
//////    {
//////        Destroy(collision.gameObject); //destroy enemy
//////        Destroy(gameObject); //destroy đạn
//////    }
//////}

//////private void OnTriggerEnter2D(Collider2D collision)
//////{
//////    // Kiểm tra nếu đối tượng có HealthController (thay vì chỉ kiểm tra EnemyMovement)
//////    var health = collision.GetComponent<HealthController>();
//////    if (health != null)
//////    {
//////        Debug.Log($"[Bullet] Gọi TakeDamage() trên {collision.name}");
//////        health.TakeDamage(_damage);

//////        // Sau khi bắn, đạn biến mất
//////        Destroy(gameObject);
//////    }
//////}
///
using UnityEngine;

public class BulletMap2 : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _overlapRadius = 0.5f; // bán kính kiểm tra collider
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        DestroyWhenOffScreen();
        CheckHit();
    }

    private void CheckHit()
    {
        // Kiểm tra tất cả collider trong radius, chỉ layer Enemy
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _overlapRadius, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Debug.Log($"[Debug] Collider: {hit.name}, Layer: {LayerMask.LayerToName(hit.gameObject.layer)}, active: {hit.gameObject.activeInHierarchy}");

            // Lấy HealthController, có thể nằm trên parent nếu collider ở child
            HealthController health = hit.GetComponent<HealthController>();
            if (health == null)
            {
                health = hit.GetComponentInParent<HealthController>();
            }

            if (health != null)
            {
                Debug.Log($"[Bullet] Hit {hit.name} at position {hit.transform.position}");
                health.TakeDamage(_damage);

                // Destroy bullet sau khi gây damage 1 target
                Destroy(gameObject);
                return;
            }
        }
    }

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }

    // Vẽ gizmo để debug overlap
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _overlapRadius);
    }
}
