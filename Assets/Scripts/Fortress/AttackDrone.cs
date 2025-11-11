using UnityEngine;
using System.Collections;

public class AttackDrone : Drone
{
    public GameObject bulletPrefab;

    private float fireTimer;

    protected override void Update()
    {
        base.Update(); // Giảm fireCooldown

        if (target != null && fireCooldown <= 0f)
        {
            Shoot();
            // Tăng thời gian cooldown thêm 50%
            fireCooldown = 1f / Mathf.Max(0.01f, fireRate) * 1.5f;
        }
    }


    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;

        Vector3 dir = (target.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        ABullet ab = bullet.GetComponent<ABullet>();
        if (ab != null) ab.Launch(dir, bulletSpeed);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}
