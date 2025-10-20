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
    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody2D>();
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
