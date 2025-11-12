using UnityEngine;

public class Drone : MonoBehaviour
{
    [Header("Drone cơ bản")]
    public Transform player;
    public float orbitRadius = 3f;
    public float orbitSpeed = 60f;
    public float followSmooth = 5f;
    [HideInInspector]
    public float orbitAngle = 0f; // có thể set từ SimpleDroneSwitcher

    [Header("Targeting & Shooting")]
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 3f;
    public float targetRange = 8f;

    protected Transform target;
    public float fireCooldown = 0f;
    public float minDistanceFromPlayer = 1f;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        InvokeRepeating(nameof(FindTarget), 0f, 0.25f);
    }

    protected virtual void Update()
    {
        if (player == null) return;

        OrbitAroundPlayer();
        RotateToTarget();
        fireCooldown -= Time.deltaTime;
    }

    void OrbitAroundPlayer()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        float rad = orbitAngle * Mathf.Deg2Rad;

        Vector3 desiredPos = player.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

        Vector3 dirToPlayer = desiredPos - player.position;
        if (dirToPlayer.magnitude < minDistanceFromPlayer)
            desiredPos = player.position + dirToPlayer.normalized * minDistanceFromPlayer;

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSmooth);
    }

    void RotateToTarget()
    {
        Vector3 lookDir = (target != null) ? (target.position - transform.position) : (player.position - transform.position);
        if (lookDir.magnitude < 0.1f)
            lookDir = Vector3.up;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (player != null)
            Gizmos.DrawWireSphere(player.position, orbitRadius);
    }
}
