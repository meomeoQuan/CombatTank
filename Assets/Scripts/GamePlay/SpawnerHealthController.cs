using UnityEngine;
using UnityEngine.Events;

public class SpawnerHealthController : MonoBehaviour
{
    [SerializeField]
    private float _currentHealth;

    [SerializeField]
    private float _maximumHealth;

    public float RemainingHealthPercentage
    {
        get
        {
            return _currentHealth / _maximumHealth;
        }
    }

    public bool IsInvincible { get; set; } // cho phép bất khả chiến bại sau khi nhận sát thương

    public UnityEvent OnDied;    // sự kiện khi spawner chết
    public UnityEvent OnDamaged; // sự kiện khi spawner nhận sát thương

    public void TakeDamage(float damageAmount)
    {
        Debug.Log($"[SpawnerHealth] {name} nhận {damageAmount} damage");

        if (_currentHealth == 0)
            return;

        if (IsInvincible)
            return;

        _currentHealth -= damageAmount;
        Debug.Log($"[SpawnerHealth] Máu còn lại: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Debug.Log("[SpawnerHealth] Gọi OnDied");
            OnDied.Invoke();
        }
        else
        {
            OnDamaged.Invoke();
        }
    }

    public void AddHealth(float amountToAdd)
    {
        if (_currentHealth == _maximumHealth)
            return;

        _currentHealth += amountToAdd;

        if (_currentHealth > _maximumHealth)
        {
            _currentHealth = _maximumHealth;
        }
    }
}
