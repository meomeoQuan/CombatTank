using UnityEngine;
using System.Collections;
using Mono.Cecil.Cil;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyHealthController Healthbar;  // ← ADD THIS

    private Rigidbody2D rb;
    private Animator animator;

    [Header("Stats")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float maxHP = 50f;
    private float currentHP;
    private bool isDead = false;
    private bool isAttacking = false;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackAnimDuration = 2f;
    private float lastAttackTime = 0f;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 scaleFacingRight = new Vector3(3, 3, 1);
    [SerializeField] private Vector3 scaleFacingLeft = new Vector3(-3, 3, 1);

    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHP = maxHP;

        // Initialize healthbar
        UpdateHealthbar();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time - lastAttackTime >= attackCooldown && !isAttacking)
            {
                StartCoroutine(AttackRoutine());
                lastAttackTime = Time.time;
            }
        }
        else if (!isAttacking)
        {
            MoveTowardsPlayer();
        }

        // Flip sprite
        if (player.position.x > transform.position.x)
            transform.localScale = scaleFacingRight;
        else
            transform.localScale = scaleFacingLeft;
    }

    void FixedUpdate()
    {
        if (isDead || isAttacking) return;
        rb.linearVelocity = moveDirection * speed;
    }

    private void MoveTowardsPlayer()
    {
        animator.SetBool("IsWalking", true);
        Vector3 direction = (player.position - transform.position).normalized;
        moveDirection = direction;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsAttack01", true);

        Debug.Log("<color=orange>[Enemy]</color> ⚔️ Attack started!");

        yield return new WaitForSeconds(attackAnimDuration * 0.5f);
        DealDamage();

        yield return new WaitForSeconds(attackAnimDuration * 0.5f);

        animator.SetBool("IsAttack01", false);
        animator.SetBool("IsWalking", true);
        isAttacking = false;

        Debug.Log("<color=orange>[Enemy]</color> Attack finished.");
    }

    private void DealDamage()
    {
        Debug.Log($"<color=red>[Enemy]</color> Attacked player for {damage} damage!");
        // TODO: player HP logic here
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.TakeDamage(damage);
        }else
        {
            Debug.LogWarning("<color=red>[Enemy]</color> PlayerMovement component not found on player!");
        }
        
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;
        UpdateHealthbar();  // ← UPDATE HEALTHBAR
        animator.SetBool("IsGetHit", true);

        Debug.Log($"<color=yellow>[Enemy]</color> Got hit! HP: {currentHP:F1}/{maxHP}");

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("IsDie", true);
        rb.linearVelocity = Vector2.zero;

        // Hide healthbar on death
        if (Healthbar != null)
            Healthbar.gameObject.SetActive(false);

        Debug.Log("<color=gray>[Enemy]</color> Skeleton is dead 💀");

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1.8f);
    }

    // === HEALTHBAR SUPPORT ===
    private void UpdateHealthbar()
    {
        if (Healthbar != null)
        {
            Healthbar.SetHealth(currentHP, maxHP);
        }
    }
}