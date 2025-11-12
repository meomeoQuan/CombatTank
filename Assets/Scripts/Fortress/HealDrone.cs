using UnityEngine;
using System.Collections;

public class HealDrone : Drone
{
    [Header("🔹 Thuộc tính hồi máu")]
    public float healAmount = 10f;    // Lượng HP hồi mỗi lần
    public float healRate = 1f;       // Lần hồi mỗi giây
    public float healRange = 3f;      // Bán kính hồi HP

    private float healCooldown = 0f;

    protected override void Update()
    {
        base.Update();

        healCooldown -= Time.deltaTime;
        if (healCooldown <= 0f)
        {
            Heal();
            healCooldown = 1f / Mathf.Max(0.01f, healRate);
        }
    }

    void Heal()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, healRange);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            HealthController hc = hit.GetComponent<HealthController>();
            if (hc != null && hc.RemainingHealthPercentage < 1f) // Chỉ heal khi máu chưa đầy
            {
                hc.AddHealth(healAmount);
                Debug.Log($"💚 {hit.name} được hồi {healAmount} HP từ {gameObject.name} | Máu hiện tại: {hc.CurrentHealth}/{hc.MaximumHealth}");
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
