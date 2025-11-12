using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField]
    private float _currentHealth;

    [SerializeField]
    private float _maximumHealth;

    public float CurrentHealth => _currentHealth;
    public float MaximumHealth => _maximumHealth;

    public float RemainingHealthPercentage => _currentHealth / _maximumHealth;

    public bool IsInvincible { get; set; } // cho phép bất khả chiến bại

    public UnityEvent OnDied;    // Khi hết máu
    public UnityEvent OnDamaged; // Khi nhận sát thương

    // ================== Take Damage ==================
    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth == 0)
        {
            Debug.Log($"{gameObject.name} đã hết máu — không thể nhận thêm sát thương.");
            return;
        }

        if (IsInvincible)
        {
            Debug.Log($"{gameObject.name} đang bất tử — bỏ qua sát thương.");
            return;
        }

        _currentHealth -= damageAmount;
        if (_currentHealth < 0) _currentHealth = 0;

        Debug.Log($"🩸 {gameObject.name} nhận {damageAmount} sát thương, máu còn: {_currentHealth}/{_maximumHealth}");

        if (_currentHealth == 0)
        {
            Debug.Log($"☠ {gameObject.name} đã chết!");
            OnDied?.Invoke();
        }
        else
        {
            OnDamaged?.Invoke();
        }
    }

    // ================== Add Health ==================
    public void AddHealth(float amountToAdd)
    {
        if (_currentHealth >= _maximumHealth)
        {
            Debug.Log($"{gameObject.name} máu đã đầy, không cần hồi.");
            return;
        }

        _currentHealth += amountToAdd;
        if (_currentHealth > _maximumHealth)
            _currentHealth = _maximumHealth;

        Debug.Log($"💚 {gameObject.name} được hồi {amountToAdd} HP, máu hiện tại: {_currentHealth}/{_maximumHealth}");
    }
}
