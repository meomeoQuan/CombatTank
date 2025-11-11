using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float _currentHealth = 100f;
    [SerializeField] private float _maximumHealth = 100f;

    public Image fillBar;  // Drag FillBar here (or auto-find)

    public float RemainingHealthPercentage => _currentHealth / _maximumHealth;
    public bool IsInvincible { get; set; }
    public UnityEvent OnDied, OnDamaged;

    private void Start()
    {
        // AUTO-FIND fillBar if not assigned
        if (fillBar == null)
        {
            fillBar = GetComponentInChildren<Image>();
            if (fillBar != null)
                Debug.Log($"[HealthController] Auto-found fillBar: {fillBar.name}");
            else
                Debug.LogError("[HealthController] No Image found in children!");
        }

        _currentHealth = _maximumHealth;
        UpdateHealthBar();
    }

    public void UpdateHealth(int currentHP, int maxHP)
    {
        _currentHealth = currentHP;
        _maximumHealth = maxHP;

        if (fillBar == null)
        {
            Debug.LogError("fillBar is NULL! Assign FillBar Image.", this);
            return;
        }

        float percentage = (float)currentHP / (float)maxHP;
        fillBar.fillAmount = percentage;
        fillBar.color = Color.Lerp(Color.red, Color.green, percentage);

        Debug.Log($"[HealthController] Updated fillBar to {percentage:P0} ({currentHP}/{maxHP})");
    }

    private void UpdateHealthBar()
    {
        if (fillBar == null) return;
        float percentage = RemainingHealthPercentage;
        fillBar.fillAmount = percentage;
        fillBar.color = Color.Lerp(Color.red, Color.green, percentage);
    }

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth <= 0 || IsInvincible) return;

        _currentHealth -= damageAmount;
        if (_currentHealth < 0) _currentHealth = 0;

        UpdateHealthBar();

        if (_currentHealth <= 0) OnDied.Invoke();
        else OnDamaged.Invoke();
    }

    public void AddHealth(float amountToAdd)
    {
        _currentHealth = Mathf.Min(_currentHealth + amountToAdd, _maximumHealth);
        UpdateHealthBar();
    }
}