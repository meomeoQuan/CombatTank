using UnityEngine;
using System.Linq;

public class FreezeDrone : Drone
{
    [Header("❄️ Đóng băng kẻ địch")]
    public float effectRadius = 4f;        // Bán kính ảnh hưởng
    public float freezeDuration = 2f;      // Thời gian đứng yên
    public float applyInterval = 1f;       // Chu kỳ quét (giây)
    public int maxFrozenEnemies = 2;       // Giới hạn số enemy có thể bị đóng băng
    public LayerMask enemyLayer = ~0;      // ✅ Mặc định quét tất cả layer (nếu chưa set)

    private float _nextApplyTime;

    protected override void Update()
    {
        base.Update();

        if (Time.time >= _nextApplyTime)
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
            return; // Không log nữa nếu không có enemy hợp lệ

        foreach (var e in enemies)
        {
            EnemyMovement enemy = e.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.Freeze(freezeDuration);
            }
        }

        // ✅ Chỉ log một lần cho tổng số enemy bị đóng băng
        //Debug.Log($"❄️ {name} đóng băng {enemies.Count} enemy trong {freezeDuration}s");
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
