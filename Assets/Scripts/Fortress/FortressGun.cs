using UnityEngine;
using System.Collections;

public class FortressGun : MonoBehaviour
{
    [Header("Cài đặt súng")]
    public Transform firePoint;
    public GameObject energyShotPrefab;
    public float fireRate = 1f;
    public float range = 6f;

    [Header("Cài đặt bắn nhiều lần")]
    public int burstCount = 3;
    public float burstDelay = 0.2f;

    [Header("Cài đặt băng đạn")]
    public int magazineSize = 15;
    public float reloadTime = 2f;

    [Header("Cài đặt máu Fortress")]
    public int maxHP = 100;
    private int currentHP;

    private int currentAmmo;
    private bool isReloading = false;
    private float fireCooldown = 0f;
    private Transform target;
    private bool isBursting = false;
    private Animator animator;

    void Start()
    {
        currentAmmo = magazineSize;
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        InvokeRepeating(nameof(FindTarget), 0f, 0.25f);
    }

    void Update()
    {
        if (isReloading || currentHP <= 0) return;
        if (target == null) return;

        RotateToTarget();
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f && !isBursting)
        {
            if (currentAmmo > 0)
            {
                StartCoroutine(BurstFire());
                fireCooldown = 1f / fireRate;
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
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
        if (energyShotPrefab == null || firePoint == null || target == null) return;

        GameObject bulletGO = Instantiate(energyShotPrefab, firePoint.position, Quaternion.identity);
        ABullet shotComp = bulletGO.GetComponent<ABullet>();
        if (shotComp != null)
        {
            Vector2 shootDir = (target.position - firePoint.position).normalized;
            shotComp.Launch(shootDir, shotComp.speed, shotComp.damage);
        }
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break;

        isReloading = true;
        Debug.Log($"{gameObject.name} đang nạp đạn...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;
        Debug.Log($"{gameObject.name} đã nạp lại đạn xong!");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ✅ Nếu trúng đạn của tank
        if (collision.CompareTag("Bullet"))
        {
            var bullet = collision.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.Damage);
                bullet.SendMessage("Explode", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        if (currentHP <= 0) return;

        currentHP -= dmg;
        Debug.Log($"{name} trúng đạn! Mất {dmg} máu, còn {currentHP} HP");

        if (animator != null)
        {
            animator.SetTrigger("Hit"); // nếu có animation bị bắn
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} đã bị phá huỷ!");
        if (animator != null)
            animator.SetBool("isDead", true); // hoặc hiệu ứng nổ

        // Ngưng bắn
        StopAllCoroutines();
        CancelInvoke();

        // Tắt súng
        this.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
