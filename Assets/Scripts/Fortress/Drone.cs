using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour
{
    [Header("🎮 Cài đặt mục tiêu bay")]
    public Transform player;          // 🧩 Player để bay quanh
    public float orbitRadius = 3f;    // 🌀 Bán kính bay vòng quanh Player
    public float orbitSpeed = 60f;    // 🌀 Tốc độ bay vòng quanh (độ/giây)
    public float followSmooth = 5f;   // 🧲 Độ mượt khi theo Player (nếu Player di chuyển)

    [Header("🔫 Cài đặt bắn")]
    public GameObject bulletPrefab;   // 💥 Prefab đạn (chính là ABullet)
    public Transform firePoint;       // 🎯 Vị trí bắn ra đạn
    public float fireRate = 1f;       // 🔁 Số viên/giây
    public int burstCount = 1;        // 🔫 Số viên trong mỗi lượt bắn (0 = 1 viên)
    public float burstDelay = 0.15f;  // ⏱️ Delay giữa các viên trong burst
    public float bulletSpeed = 12f;   // 🚀 Tốc độ bay của đạn
    public int bulletDamage = 10;     // 💥 Sát thương của đạn
    public float targetRange = 8f;    // 🔍 Phạm vi phát hiện enemy

    [Header("🎨 Hiệu ứng & Animation (tùy chọn)")]
    public Animator animator;

    // ==================== NỘI BỘ ====================
    private float orbitAngle = 0f;
    private Transform target;
    private float fireCooldown = 0f;
    private bool isBursting = false;

    // ==================== HÀM CHÍNH ====================
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        InvokeRepeating(nameof(FindTarget), 0f, 0.25f); // 🔁 Quét mục tiêu liên tục
    }

    void Update()
    {
        if (player == null) return;

        OrbitAroundPlayer();
        RotateToTarget();

        fireCooldown -= Time.deltaTime;

        if (target != null && fireCooldown <= 0f)
        {
            if (burstCount > 1 && !isBursting)
                StartCoroutine(BurstFire());
            else
                Shoot();

            fireCooldown = 1f / Mathf.Max(0.1f, fireRate);
        }
    }

    // ==================== CHUYỂN ĐỘNG ====================
    void OrbitAroundPlayer()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;

        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector3 desiredPos = player.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmooth); // Mượt hơn khi Player di chuyển
    }

    void RotateToTarget()
    {
        Vector3 lookDir;

        if (target != null)
            lookDir = target.position - transform.position;
        else
            lookDir = player.position - transform.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // ==================== TÌM MỤC TIÊU ====================
    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetRange);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("FortressGun"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }

        target = closestEnemy;
    }

    // ==================== BẮN ĐẠN ====================
    IEnumerator BurstFire()
    {
        isBursting = true;
        for (int i = 0; i < burstCount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(burstDelay);
        }
        isBursting = false;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        ABullet bullet = bulletGO.GetComponent<ABullet>();
        if (bullet != null)
        {
            Vector2 dir = (target != null)
                ? (target.position - firePoint.position).normalized
                : transform.up; // Nếu không có target thì bắn thẳng hướng nhìn

            bullet.Launch(dir, bulletSpeed, bulletDamage);
        }

        if (animator != null)
            animator.SetTrigger("Shoot");
    }

    // ==================== GIZMOS ====================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (player != null)
            Gizmos.DrawWireSphere(player.position, orbitRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}
