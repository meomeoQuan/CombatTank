using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _speed;              // tốc độ hiện tại
    private float _baseSpeed;                           // tốc độ gốc
    [SerializeField] private float _rotationSpeed;
    private Rigidbody2D _rigibody;
    private PlayerAwarenessController _playerAwarenessController;
    private Vector2 _targetDirection;
    private float _changeDirectionCoolDown;

    [SerializeField] private float _screenBorder;
    [SerializeField] private float _obstacleCheckCircleRadius;
    [SerializeField] private float _obstacleCheckDistance;
    [SerializeField] private LayerMask _obstacleLayerMask;

    private RaycastHit2D[] _obstacleCollisions;
    private Vector2 _obstacleAvoidanceTargetDirection;
    private float _obstacleAvoidanceCoolDown;
    private Camera _camera;

    private Coroutine _statusCoroutine; // dùng chung cho slow hoặc freeze
    private bool _isFrozen = false;

    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _targetDirection = transform.up;
        _camera = Camera.main;
        _obstacleCollisions = new RaycastHit2D[10];
        _baseSpeed = _speed;
    }

    private void FixedUpdate()
    {
        // 🚫 Nếu đang bị đóng băng thì không di chuyển
        if (_isFrozen)
        {
            _rigibody.linearVelocity = Vector2.zero;
            return;
        }

        UpdateTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        HandleRandomDirectionChange();
        HandlePlayerTargeting();
        HandleObstacles();
        HandleEnemyOffScreen();
    }

    private void HandleRandomDirectionChange()
    {
        _changeDirectionCoolDown -= Time.fixedDeltaTime;
        if (_changeDirectionCoolDown < 0)
        {
            float angleChange = Random.Range(-90f, 90f);
            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
            _targetDirection = rotation * _targetDirection;
            _changeDirectionCoolDown = Random.Range(1f, 5f);
        }
    }

    private void HandlePlayerTargeting()
    {
        if (_playerAwarenessController.AwareOfPlayer)
        {
            _targetDirection = _playerAwarenessController.DirectionToPlayer;
        }
    }

    private void HandleEnemyOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if ((screenPosition.x < _screenBorder && _targetDirection.x < 0)
         || (screenPosition.x > _camera.pixelWidth - _screenBorder && _targetDirection.x > 0))
        {
            _targetDirection = new Vector2(-_targetDirection.x, _targetDirection.y);
        }

        if ((screenPosition.y < _screenBorder && _targetDirection.y < 0)
         || (screenPosition.y > _camera.pixelHeight - _screenBorder && _targetDirection.y > 0))
        {
            _targetDirection = new Vector2(_targetDirection.x, -_targetDirection.y);
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
            var obstacleCollision = _obstacleCollisions[index];
            if (obstacleCollision.collider.gameObject == gameObject)
                continue;

            if (_obstacleAvoidanceCoolDown <= 0)
            {
                _obstacleAvoidanceTargetDirection = obstacleCollision.normal;
                _obstacleAvoidanceCoolDown = 0.5f;
            }

            var targetRotation = Quaternion.LookRotation(transform.forward, _obstacleAvoidanceTargetDirection);
            var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            _targetDirection = rotation * Vector2.up;
            break;
        }
    }

    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        _rigibody.SetRotation(newRotation);
    }

    private void SetVelocity()
    {
        _rigibody.linearVelocity = transform.up * _speed;
    }

    // -------------------------------------------------
    // 🧊 Làm chậm
    // -------------------------------------------------
    public void ApplySlow(float slowFactor, float duration)
    {
        if (_isFrozen) return; // nếu đang bị đóng băng thì bỏ qua

        slowFactor = Mathf.Clamp01(slowFactor);
        if (_statusCoroutine != null)
            StopCoroutine(_statusCoroutine);

        _statusCoroutine = StartCoroutine(SlowRoutine(slowFactor, duration));
    }

    private IEnumerator SlowRoutine(float slowFactor, float duration)
    {
        _speed = _baseSpeed * (1f - slowFactor);
        Debug.Log($"{name} bị làm chậm còn {_speed:F2} trong {duration}s");

        yield return new WaitForSeconds(duration);

        _speed = _baseSpeed;
        _statusCoroutine = null;
        Debug.Log($"{name} đã hồi phục tốc độ {_baseSpeed:F2}");
    }

    // -------------------------------------------------
    // 🧊 Đóng băng hoàn toàn (Freeze)
    // -------------------------------------------------
    public void Freeze(float duration)
    {
        if (_statusCoroutine != null)
            StopCoroutine(_statusCoroutine);

        _statusCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        _isFrozen = true;
        _rigibody.linearVelocity = Vector2.zero;
        float oldSpeed = _speed;
        _speed = 0;

        Debug.Log($"🧊 {name} bị đóng băng trong {duration}s");

        yield return new WaitForSeconds(duration);

        _isFrozen = false;
        _speed = _baseSpeed;
        _statusCoroutine = null;
        Debug.Log($"🔥 {name} tan băng, hồi phục tốc độ {_baseSpeed:F2}");
    }
}
