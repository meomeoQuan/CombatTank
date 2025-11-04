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

<<<<<<< HEAD
    //public void TakeDamage(float damageAmount)
    //{
    //    if (_currentHealth == 0) //ktra lượng máu có = 0 
    //    {
    //        return;
    //    }

    //    if (IsInvincible)
    //    {
    //        return;
    //    }

    //    _currentHealth -= damageAmount;

    //    if (_currentHealth < 0) //Tránh máu là số âm
    //    {
    //        _currentHealth = 0;
    //    }

    //    if(_currentHealth == 0)
    //    {
    //        OnDied.Invoke();
    //    }

    //    else
    //    {
    //        OnDamaged.Invoke();
    //    }



    //}

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth == 0)
        {
            Debug.Log($"{gameObject.name} đã hết máu — không thể nhận thêm sát thương.");
=======
    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth == 0) //ktra lượng máu có = 0 
        {
>>>>>>> quan
            return;
        }

        if (IsInvincible)
        {
<<<<<<< HEAD
            Debug.Log($"{gameObject.name} đang bất tử — bỏ qua sát thương.");
=======
>>>>>>> quan
            return;
        }

        _currentHealth -= damageAmount;
<<<<<<< HEAD
        if (_currentHealth < 0) _currentHealth = 0;

        Debug.Log($"🩸 {gameObject.name} nhận {damageAmount} sát thương, máu còn: {_currentHealth}");

        if (_currentHealth == 0)
        {
            Debug.Log($"☠ {gameObject.name} đã chết!");
            OnDied.Invoke();
        }
=======

        if (_currentHealth < 0) //Tránh máu là số âm
        {
            _currentHealth = 0;
        }

        if(_currentHealth == 0)
        {
            OnDied.Invoke();
        }

>>>>>>> quan
        else
        {
            OnDamaged.Invoke();
        }
<<<<<<< HEAD
    }

=======

    }
>>>>>>> quan

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
