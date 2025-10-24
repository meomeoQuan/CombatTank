using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HealthController))]
public class FortressGun : MonoBehaviour
{
    [Header("⚙️ Cài đặt súng")]
    public Transform firePoint;                  // 🧩 Vị trí bắn ra đạn
    public GameObject energyShotPrefab;          // 🧩 Prefab đạn bắn ra
    public float fireRate = 1f;                  // 🧩 Số viên/giây
    public float range = 6f;                     // 🧩 Phạm vi phát hiện Player

    [Header("🎯 Cài đặt bắn loạt (bỏ burst nếu chỉ bắn 1 viên)")]
    public int burstCount = 1;                   // 🧩 Số viên mỗi đợt bắn
    public float burstDelay = 0.2f;              // 🧩 Thời gian delay giữa mỗi viên trong burst

    [Header("🔫 Cài đặt băng đạn")]
    public int magazineSize = 15;                // 🧩 Dung lượng băng đạn
    public float reloadTime = 2f;                // 🧩 Thời gian nạp lại đạn

    [Header("💥 Hiệu ứng khi phá huỷ")]
    public GameObject explosionEffect;           // 🧩 Hiệu ứng nổ khi chết

    // ========== Biến nội bộ ==========
    private int currentAmmo;                     // 🧩 Số viên còn lại
    private bool isReloading = false;            // 🧩 Đang nạp lại hay không
    private float fireCooldown = 0f;             // 🧩 Thời gian chờ giữa các lần bắn
    private Transform target;                    // 🧩 Mục tiêu hiện tại
    private bool isBursting = false;             // 🧩 Có đang trong chế độ bắn loạt không
    private Animator animator;                   // 🧩 Animator để chơi animation
    private HealthController health;             // 🧩 Quản lý máu
    private bool isDead = false;                 // 🧩 Đã chết hay chưa

    // ========== KHỞI TẠO ==========
    void Awake()
    {
        health = GetComponent<HealthController>();     // 🧩 Lấy HealthController
        animator = GetComponent<Animator>();           // 🧩 Lấy Animator nếu có
    }

    void Start()
    {
        currentAmmo = magazineSize;                    // 🧩 Đặt lại đạn ban đầu

        // 🧩 Gắn sự kiện cho HealthController
        if (health != null)
        {
            health.OnDied.AddListener(Die);            // 🧩 Khi chết thì gọi Die()
            health.OnDamaged.AddListener(OnDamagedFeedback); // 🧩 Khi bị trúng đạn gọi OnDamagedFeedback()
        }

        InvokeRepeating(nameof(FindTarget), 0f, 0.25f); // 🧩 Liên tục tìm player gần nhất
    }

    void OnDisable()
    {
        // 🧩 Gỡ listener khi bị disable để tránh memory leak
        if (health != null)
        {
            health.OnDied.RemoveListener(Die);
            health.OnDamaged.RemoveListener(OnDamagedFeedback);
        }
    }
    void Update()
    {
        if (isDead || isReloading) return;             // 🧩 Ngưng hoạt động khi chết hoặc đang reload
        if (target == null) return;                    // 🧩 Không có mục tiêu thì không bắn

        RotateToTarget();                              // 🧩 Quay hướng nòng về phía Player
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            if (currentAmmo > 0)
            {
                if (burstCount <= 1)                   // 🧩 Nếu chỉ bắn 1 viên/lượt
                {
                    Shoot();
                    currentAmmo--;
                }
                else if (!isBursting)                  // 🧩 Nếu bắn loạt
                {
                    StartCoroutine(BurstFire());
                }

                fireCooldown = 1f / Mathf.Max(0.0001f, fireRate);
            }
            else
            {
                StartCoroutine(Reload());              // 🧩 Hết đạn → reload
            }
        }
    }
    void FindTarget()
    {
        if (isDead) { target = null; return; }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))              // 🧩 Chỉ tìm đối tượng có tag Player
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }

        target = closestEnemy;                         // 🧩 Cập nhật target
    }
    void RotateToTarget()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    IEnumerator BurstFire()
    {
        isBursting = true;

        for (int i = 0; i < burstCount; i++)
        {
            if (isDead) break;

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                break;
            }

            Shoot();
            currentAmmo--;
            yield return new WaitForSeconds(burstDelay);
        }

        isBursting = false;
    }
    void Shoot()
    {
        if (isDead || energyShotPrefab == null || firePoint == null || target == null) return;

        GameObject bulletGO = Instantiate(energyShotPrefab, firePoint.position, Quaternion.identity);
        ABullet shotComp = bulletGO.GetComponent<ABullet>();
        if (shotComp != null)
        {
            Vector2 shootDir = (target.position - firePoint.position).normalized;
            shotComp.Launch(shootDir, shotComp.speed, shotComp.damage);
        }

        if (animator != null) animator.SetTrigger("Shoot"); // 🧩 Kích hoạt animation bắn
    }
    IEnumerator Reload()
    {
        if (isReloading || isDead) yield break;

        isReloading = true;
        Debug.Log($"{gameObject.name} đang nạp đạn...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log($"{gameObject.name} đã nạp lại đạn xong!");
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"⚡ FortressGun chạm với: {collision.name}");
        if (isDead || health == null) return;
        if (collision.GetComponent<BulletMap2>()) // 🧩 Nếu trúng đạn BulletMap2
        {
            health.TakeDamage(10);                     // 🧩 Giảm máu FortressGun 10 HP
            Destroy(collision.gameObject);             // 🧩 Hủy viên đạn
        }
    }
    private void OnDamagedFeedback()
    {
        if (animator != null)
            animator.SetTrigger("Hit");                // 🧩 Kích hoạt animation bị bắn
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} đã bị phá huỷ!");

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity); // 🧩 Hiệu ứng nổ

        StopAllCoroutines();
        CancelInvoke();

        if (animator != null)
            animator.SetBool("isDead", true);          // 🧩 Animation chết

        this.enabled = false;                          // 🧩 Dừng mọi hoạt động FortressGun
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
