using UnityEngine;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.DataController; // để dùng DataController

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public string myBulletTag = "Shell1";
    public OwnerType owner = OwnerType.CharacterA; // chọn A hay B
    public int Damage { get; private set; }

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isExploding = false;

    private float LiveAvailableBulletRangeX1 = 9.0f;
    private float LiveAvailableBulletRangeX2 = -9.0f;
    private float LiveAvailableBulletRangeY1 = 5.0f;
    private float LiveAvailableBulletRangeY2 = -5.0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

       void Update()
    {
        // Kiểm tra bullet ra khỏi range
        if (   rb.position.x > LiveAvailableBulletRangeX1
            || rb.position.y > LiveAvailableBulletRangeY1
            || rb.position.x < LiveAvailableBulletRangeX2
            || rb.position.y < LiveAvailableBulletRangeY2 )
        {
            Debug.Log("Bullet out of range -> destroy");
            ReturnToPool();
        }
    }

    void OnEnable()
    {
        // Reset trạng thái mỗi lần spawn
        isExploding = false;
        if (animator != null) animator.SetBool("isExploding", false);
        if (col != null) col.enabled = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Gán Damage dựa trên owner
        SetDamage();
    }

    private void SetDamage()
    {
        if (DataController.Characters == null || DataController.Characters.Count < 2)
        {
            Damage = 0;
            return;
        }

        switch (owner)
        {
            case OwnerType.CharacterA:
                Damage = DataController.Characters[0].ATK;
                break;
            case OwnerType.CharacterB:
                Damage = DataController.Characters[1].ATK;
                break;
        }

        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isExploding)
        {
            Debug.Log("Bullet hit: " + collision.name + $" | Damage: {Damage}");
            Explode();
        }
    }

    private void Explode()
    {
        isExploding = true;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;

        if (animator != null) animator.SetBool("isExploding", true);
    }

    // Gọi ở cuối animation nổ (Animation Event)
    public void OnExplosionEnd()
    {
        Debug.Log("Animation Event: Explosion End");
        isExploding = false;
        ReturnToPool();
    }

    void OnBecameInvisible()
    {
        ReturnToPool();
       
    }

    private void ReturnToPool()
    {
        Destroy(gameObject); 
    }
}

public enum OwnerType
{
    CharacterA,
    CharacterB
}
