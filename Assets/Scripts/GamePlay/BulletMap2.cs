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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // === TYPE 1: EnemyMovement (Top-down) ===
        if (collision.TryGetComponent<EnemyMovement>(out var enemyMovement))
        {
            Debug.Log($"<color=green>[Bullet]</color> Hit <b>{collision.gameObject.name}</b> (Top-down) for <b>{_damageAmount}</b> damage!");
            enemyMovement.TakeHit(_damageAmount);
            Destroy(gameObject);
            return;
        }

        // === TYPE 2: Enemy (Platformer Skeleton) ===
        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            Debug.Log($"<color=green>[Bullet]</color> Hit <b>{collision.gameObject.name}</b> (Skeleton) for <b>{_damageAmount}</b> damage!");
            enemy.TakeDamage(_damageAmount);  // ✅ Use TakeDamage for Enemy!
            Destroy(gameObject);
            return;
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
}