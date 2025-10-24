using UnityEngine;

public class BulletMap2 : MonoBehaviour
{
        [SerializeField] private float _damageAmount = 25f; 
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        DestroyWhenOffScreen();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void OnTriggerEnter2D(Collider2D collision)
{
    // Check if it’s an enemy (has either EnemyMovement or Enemy component)
    EnemyMovement enemyMove = collision.GetComponent<EnemyMovement>();
    Enemy enemy = collision.GetComponent<Enemy>();

    if (enemyMove != null || enemy != null)
    {
        // Print the enemy’s name for debugging
        Debug.Log($"<color=red>[Bullet]</color> Hit and destroyed: <b>{collision.gameObject.name}</b>");

        // Destroy the enemy
        Destroy(collision.gameObject);

        // Destroy the bullet
        Destroy(gameObject);
    }
}

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position); //xác định vị trí đạn 

        if (screenPosition.x < 0 ||
           screenPosition.x > _camera.pixelWidth ||
           screenPosition.y < 0 ||
           screenPosition.y > _camera.pixelHeight)
        {
            Destroy(gameObject);
        }
    }
}
