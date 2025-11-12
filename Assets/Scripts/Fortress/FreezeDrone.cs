using UnityEngine;
using System.Linq;

public class FreezeDrone : Drone
{
    [Header("❄️ Đóng băng kẻ địch")]
    public float effectRadius = 4f;        // Bán kính ảnh hưởng
    public float freezeDuration = 2f;      // Thời gian đứng yên
    public float applyInterval = 1f;       // Chu kỳ quét kiểm tra (giây)
    public int maxFrozenEnemies = 2;       // Giới hạn số enemy có thể bị đóng băng
    public float freezeCooldown = 10f;      // ⏳ Thời gian hồi sau khi đóng băng
    public LayerMask enemyLayer = ~0;      // Mặc định quét tất cả layer

    private float _nextApplyTime;          // Thời gian lần quét tiếp theo
    private bool _onCooldown = false;      // Đang trong thời gian hồi

    protected override void Update()
    {
        base.Update();

        if (Time.time >= _nextApplyTime && !_onCooldown)
        {
            FreezeNearbyEnemies();
            _nextApplyTime = Time.time + applyInterval;
        }
    }

    void FreezeNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, effectRadius, enemyLayer);

        // Lọc ra enemy thực sự có EnemyMovement
        var enemies = hits
            .Where(h => h.CompareTag("Enemy") && h.GetComponent<EnemyMovement>() != null)
            .OrderBy(h => Vector2.Distance(transform.position, h.transform.position))
            .Take(maxFrozenEnemies)
            .ToList();

        if (enemies.Count == 0)
            return;

        foreach (var e in enemies)
        {
            EnemyMovement enemy = e.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.Freeze(freezeDuration);
            }
        }

        // 🔒 Kích hoạt cooldown sau khi đã đóng băng kẻ địch
        _onCooldown = true;
        Invoke(nameof(ResetCooldown), freezeCooldown);

    }

    void ResetCooldown()
    {
        _onCooldown = false;
        Debug.Log($"✅ {name} đã hồi xong, có thể đóng băng lại");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
