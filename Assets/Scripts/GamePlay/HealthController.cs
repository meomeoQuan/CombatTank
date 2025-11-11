using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
// User health controller to manage health, damage, and death
public class HealthController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private float _currentHealth;

    [SerializeField]
    private float _maximumHealth;

    public Image fillBar;
    // public TextMeshProUGUI healthText;

    public float RemainingHealthPercentage
    {
        get
        {
            return _currentHealth / _maximumHealth;
        }
    }

    public bool IsInvincible { get; set; } //cho phép bất khả chiến bại sau mỗi lần nhận sát thương 

    public UnityEvent OnDied; //gọi sự kiện này khi hết máu -> chết
    public UnityEvent OnDamaged; //nhận sát thương để kích hoạt bất khả chiến bại 

public void UpdateHealth(int currentHP, int maxHP)
{
    if (fillBar == null)
    {
        Debug.LogError("fillBar is NULL! Assign FillBar Image in HealthController.", this);
        return;
    }

    fillBar.fillAmount = (float)currentHP / (float)maxHP;
    Debug.Log($"HealthController: Updated fillBar to {fillBar.fillAmount}");  // ← ADD THIS
}

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth == 0) //ktra lượng máu có = 0 
        {
            return;
        }

        if (IsInvincible)
        {
            return;
        }

        _currentHealth -= damageAmount;

        if (_currentHealth < 0) //Tránh máu là số âm
        {
            _currentHealth = 0;
        }

        if (_currentHealth == 0)
        {
            OnDied.Invoke();
        }

        else
        {
            OnDamaged.Invoke();
        }

    }

    public void AddHealth(float amountToAdd) //thêm máu
    {
        if (_currentHealth == _maximumHealth) //nếu đã đạt máu tối đa thì không cần làm gì cả
        {
            return;
        }
        _currentHealth += amountToAdd;

        if (_currentHealth > _maximumHealth)  //nếu máu hiện tại lớn hơn máu tối đa
        {
            _currentHealth = _maximumHealth; //đặt máu hiện tại = máu tối đa
        }
    }
}
