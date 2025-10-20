using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private float _speed; //tốc độ của kẻ thù

    [SerializeField]
    private float _rotationSpeed;
    private Rigidbody2D _rigibody; //muốn di chuyển kẻ thù
    private PlayerAwarenessController _playerAwarenessController;
    private Vector2 _targetDriection; //hướng mục tiêu

    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateTargetDirection(); 
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection() //cập nhật hướng mục tiêu 
    {
        if (_playerAwarenessController.AwareOfPlayer) //ktra xem enemy có nhận được player không
        {
            _targetDriection = _playerAwarenessController.DirectionToPlayer; //nếu có thì hướng mục tiêu theo hướng của player
        }
        else
        {
            _targetDriection = Vector2.zero; //nếu không thì hướng mục tiêu là 0
        }
    }

    private void RotateTowardsTarget() //xoay hướng về phía mục tiêu
    {  
        //kiểm tra trước xem hướng mục tiêu có bằng 0 không 
        if(_targetDriection == Vector2.zero)
        {
            return; //nếu bằng 0 thì không làm gì cả
        }
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDriection); //tạo một quaternion để xoay về hướng mục tiêu
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime); //tạo một quaternion mới để xoay từ vị trí hiện tại đến vị trí mục tiêu với tốc độ xoay
        _rigibody.SetRotation(newRotation); //đặt vị trí xoay mới cho rigidbody
    }

    private void SetVelocity() //đặt vận tốc
    {
        if(_targetDriection == Vector2.zero)
        {
            _rigibody.linearVelocity = Vector2.zero; 
        }
        else
        {
            _rigibody.linearVelocity = transform.up * _speed;
        }
    }
}
