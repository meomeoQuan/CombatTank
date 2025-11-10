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
    private float _changeDirectionCoolDown; //lưu trữ tgian còn lại trước khi đổi hướng tiếp theo 
    [SerializeField]
    private float _screenBorder;
    [SerializeField]
    private float _obstacleCheckCircleRadius;
    [SerializeField]
    private float _obstacleCheckDistance;
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    private RaycastHit2D[] _obstacleCollisions;
    private Vector2 _obstacleAvoidanceTargetDirection;
    private float _obstacleAvoidanceCoolDown;
    private Camera _camera;
    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _targetDriection = transform.up; //hướng mục tiêu ban đầu sẽ là hướng hiện tại của enemy 
        _camera = Camera.main;
        _obstacleCollisions = new RaycastHit2D[10];
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
        HandleRandomDirectionChange(); //enemy sẽ đi lang thang nhưng khi tank đủ gần, sẽ ghi đè hướng ngẫu nhiên 
        HandlePlayerTargeting();
        HandleObstacles();
        HandleEnemyOffScreen();
    }

    private void HandleRandomDirectionChange() //thay đổi hướng ngẫu nhiên lúc lang thang 
    {
        _changeDirectionCoolDown -= Time.fixedDeltaTime; //giảm tgian chờ đổi hướng
        if (_changeDirectionCoolDown < 0) { 
            float angleChange = Random.Range(-90f, 90f); //tạo góc ngẫu nhiên
            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
            _targetDriection = rotation * _targetDriection; //cập nhật hướng mục tiêu mới

            _changeDirectionCoolDown = Random.Range(1f, 5f); //tgian hồi chiêu trước khi đổi hướng tiếp theo 
        }
    }    

    private void HandlePlayerTargeting() //xử lí nhắm xe tank 
    {
        if (_playerAwarenessController.AwareOfPlayer) //ktra xem enemy có nhận được player không
        {
            _targetDriection = _playerAwarenessController.DirectionToPlayer; //nếu có thì hướng mục tiêu theo hướng của player
        }
    }

    private void HandleEnemyOffScreen() //thay vì ngăn enemy ko di chuyển khi đến rìa màn hình, thì sẽ đổi hướng
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if ((screenPosition.x < _screenBorder && _targetDriection.x < 0) 
        || (screenPosition.x > _camera.pixelWidth - _screenBorder && _targetDriection.x > 0)) 
        {
            _targetDriection = new Vector2(-_targetDriection.x, _targetDriection.y); //nếu đến rìa của chiều rộng thì quay hướng bằng cách thay đổi hướng ngược lại trục x
        }

        if ((screenPosition.y < _screenBorder && _targetDriection.y < 0) 
        || (screenPosition.y > _camera.pixelHeight - _screenBorder && _targetDriection.y > 0))
        {
            _targetDriection = new Vector2(_targetDriection.x, - _targetDriection.y); //đi lạc phần trên cùng hoặc dưới cùng thì đảo ngược hướng y, giữ nguyên x
        }
    }

    private void HandleObstacles()
    {
        _obstacleAvoidanceCoolDown -= Time.deltaTime;

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(_obstacleLayerMask);

        int numberOfCollisions = Physics2D.CircleCast(
            transform.position, 
            _obstacleCheckCircleRadius,
            transform.up,
            contactFilter,
            _obstacleCollisions,
            _obstacleCheckDistance);

        for (int index = 0; index < numberOfCollisions; index++)
        { 
            var obstalceCollision = _obstacleCollisions[index];

            if(obstalceCollision.collider.gameObject == gameObject)
            {
                continue;
            }

            if (_obstacleAvoidanceCoolDown <= 0)
            {
                _obstacleAvoidanceTargetDirection = obstalceCollision.normal;
                _obstacleAvoidanceCoolDown = 0.5f;
            }

            var targetRotation = Quaternion.LookRotation(transform.forward, _obstacleAvoidanceTargetDirection);
            var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            _targetDriection = rotation * Vector2.up;
            break;
        }

    }

    private void RotateTowardsTarget() //xoay hướng về phía mục tiêu
    {   
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDriection); //tạo một quaternion để xoay về hướng mục tiêu
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime); //tạo một quaternion mới để xoay từ vị trí hiện tại đến vị trí mục tiêu với tốc độ xoay
        _rigibody.SetRotation(newRotation); //đặt vị trí xoay mới cho rigidbody
    }

    private void SetVelocity() //di chuyển theo hướng đã xoay theo vận tốc
    {
            _rigibody.linearVelocity = transform.up * _speed;
    }
}
