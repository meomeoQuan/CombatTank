using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;
    private Rigidbody2D _rigibody;
    private Vector2 _movementInput; //biến nhận input di chuyển
    private Vector2 _smoothedMovementInput;
    private Vector2 _smoothedMovementVelocity;
    private Camera _camera; //camera để biết được những thứ có và không có trên màn hình
    [SerializeField]
    private float _screenBorder; //thêm đường viền màn hình để player không thể tràn 1 xí ra khỏi màn hình 
    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        SetPlayerVelocity();
        RotateInDirectionOfInput();
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(_smoothedMovementInput, _movementInput, ref _smoothedMovementVelocity, 0.1f);
        _rigibody.linearVelocity = _smoothedMovementInput * _speed; //gán vận tốc cho rigidbody bằng với input di chuyển

        PreventPlayerGoingOffScreen(); 
    }

    private void PreventPlayerGoingOffScreen() //Vận tốc = 0 nếu player di chuyển ra khoải màn hình 
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position); //lấy vị trí của ng chơi đễ ktra

        if((screenPosition.x < _screenBorder && _rigibody.linearVelocity.x < 0) //nếu player ra khoải bên trái thì xét vận tốc = 0, giữ nguyên vị trí y hiện tại 
        || (screenPosition.x > _camera.pixelWidth - _screenBorder && _rigibody.linearVelocity.x >0)) //chiều rộng
        {
            _rigibody.linearVelocity = new Vector2(0, _rigibody.linearVelocity.y);
        }

        if ((screenPosition.y < _screenBorder && _rigibody.linearVelocity.y < 0) //bên phải
        || (screenPosition.y > _camera.pixelHeight - _screenBorder && _rigibody.linearVelocity.y > 0)) //chiều cao (phía trên cùng)
        {
            _rigibody.linearVelocity = new Vector2(_rigibody.linearVelocity.x, 0);
        }
    }

    private void RotateInDirectionOfInput() //kiểm tra đầu vào có bất kì chuyển động nào không
    {
        if(_movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _smoothedMovementInput); //tạo một quaternion để xoay về hướng mục tiêu
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime); //biến cho phép xoay 
            _rigibody.MoveRotation(rotation); //áp dụng vị trí mới cho vật thể rắn

        }
    }

    private void OnMove(InputValue inputValue) //nhận tham số từ input system
    {
        _movementInput = inputValue.Get<Vector2>(); 
    }
}
