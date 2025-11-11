using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

    public bool IsInvincible { get; set; } //cho phép bất khả chiến bại sau mỗi lần nhận sát thương 

    public UnityEvent OnDied; //gọi sự kiện này khi hết máu -> chết
    public UnityEvent OnDamaged; //nhận sát thương để kích hoạt bất khả chiến bại 

    public void TakeDamage(float damageAmount)
    {
        Debug.Log($"[Health] {name} nhận {damageAmount} damage");

        if (_currentHealth == 0) //ktra lượng máu có = 0 
        {
            return;
        }

        if (IsInvincible)
        {
            return;
        }

        _currentHealth -= damageAmount;
        Debug.Log($"[Health] Máu còn lại: {_currentHealth}");


        if (_currentHealth < 0) //Tránh máu là số âm
        {
            _currentHealth = 0;
            Debug.Log("[Health] Gọi OnDied");
            OnDied.Invoke();
        }

        if (_currentHealth == 0)
        {
            Debug.Log("[Health] Gọi OnDied");
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
