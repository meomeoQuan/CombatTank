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

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
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
        if (isExploding) return;

        Debug.Log("Bullet hit: " + collision.name + $" | Damage: {Damage}");

        //// --- Gây sát thương cho FortressGun ---
        //if (collision.CompareTag("FortressGun"))
        //{
        //    FortressGun fortress = collision.GetComponent<FortressGun>();
        //    if (fortress != null)
        //    {
        //        fortress.TakeDamage(Damage);
        //    }
        //}

        Explode();
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
