using UnityEngine;
// Enemy 2D top down 
public class EnemyAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private float _damageAmount;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>()) //ktra xem có phải người chơi va chạm không
        {
            var healthController = collision.gameObject.GetComponent<HealthController>();
            Debug.Log($"<color=red>[Enemy]</color> Attacking player, dealing <b>{_damageAmount}</b> damage!");
            healthController.TakeDamage(_damageAmount);
        }
    }
}
