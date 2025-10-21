using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootMap2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject _bulletPrefabs;
    [SerializeField]
    private float _bulletSpeed = 20f;
    [SerializeField]
    private bool _fireContinuously;
    private bool _fireSingle; //ktra phím space đã nhấn hay chưa trong khi chờ tgian hết hạn 
    [SerializeField]
    private Transform _gunOffset;
    [SerializeField]
    private float _timeBetweenShots;
    private float _lastFireTime;
    private void FireBullet()
    {
        //Instantiate tạo đạn mới, sử dụng vị trí của xe tank, rotation của xe tank
        GameObject bullet = Instantiate(_bulletPrefabs, _gunOffset.position, transform.rotation);
        Rigidbody2D rigibody = bullet.GetComponent<Rigidbody2D>();
        rigibody.linearVelocity = transform.up * _bulletSpeed; //đạn bay theo hướng lên của xe tank
    }

    // Update is called once per frame
    void Update() //kiểm tra xem có bắn liên tục không
    {
        if (_fireContinuously || _fireSingle)
        {
            float timeSinceLastFire = Time.time - _lastFireTime;
            if (timeSinceLastFire >= _timeBetweenShots) 
            {
                FireBullet();
                _lastFireTime = Time.time;
                _fireSingle = false; //reset biến bắn đơn
            }
        }
    }


    private void OnFire(InputValue inputValue)
    {
        _fireContinuously = inputValue.isPressed;
        if (inputValue.isPressed)
        {
            _fireSingle = true;
        }
    }
}
