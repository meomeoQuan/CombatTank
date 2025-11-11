using UnityEngine;
// Enemy 2D top down 
public class EnemyAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private float _damageAmount;

   private void OnCollisionEnter2D(Collision2D collision)
{
    if (!collision.gameObject.CompareTag("Player")) return;

    var player = collision.gameObject.GetComponent<PlayerMovement>();
    if (player != null)
    {
        player.TakeDamage((int)_damageAmount);
        Debug.Log($"<color=red>[Enemy]</color> Hit! Damage: {_damageAmount}");
    }
}
}
