using UnityEngine;
// enemy 2d top down movement and health
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAwarenessController))]
public class EnemyMovement : MonoBehaviour
{
    // === Movement ===
    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _rotationSpeed = 180f;
    [SerializeField] private float _screenBorder = 50f;

    // === Obstacle Avoidance ===
    [Header("Obstacle Avoidance")]
    [SerializeField] private float _obstacleCheckCircleRadius = 0.5f;
    [SerializeField] private float _obstacleCheckDistance = 1f;
    [SerializeField] private LayerMask _obstacleLayerMask;

    // === Health System ===
    [Header("Health")]
    [SerializeField] private float MaxHitpoints = 5f;
    public float Hitpoints;
    public EnemyHealthController Healthbar;

    // === Private Fields ===
    private Rigidbody2D _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Vector2 _targetDirection;
    private float _changeDirectionCooldown;
    private float _obstacleAvoidanceCooldown;
    private Vector2 _obstacleAvoidanceTargetDirection;
    private Camera _camera;
    private RaycastHit2D[] _obstacleCollisions;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _targetDirection = transform.up;
        _camera = Camera.main;
        _obstacleCollisions = new RaycastHit2D[10];
    }

    private void Start()
    {
        Hitpoints = MaxHitpoints;
        UpdateHealthbar();
    }

    private void FixedUpdate()
    {
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
        _changeDirectionCooldown -= Time.fixedDeltaTime;
        if (_changeDirectionCooldown < 0)
        {
            float angleChange = Random.Range(-90f, 90f);
            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
            _targetDirection = rotation * _targetDirection;
            _changeDirectionCooldown = Random.Range(1f, 5f);
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
        _obstacleAvoidanceCooldown -= Time.deltaTime;

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
            {
                continue;
            }

            if (_obstacleAvoidanceCooldown <= 0)
            {
                _obstacleAvoidanceTargetDirection = obstacleCollision.normal;
                _obstacleAvoidanceCooldown = 0.5f;
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
        _rigidbody.SetRotation(newRotation);
    }

    private void SetVelocity()
    {
        _rigidbody.linearVelocity = transform.up * _speed;
    }

    // === Health System ===
    public void TakeHit(float damage)
    {
        Hitpoints -= damage;
        UpdateHealthbar();

        if (Hitpoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHealthbar()
    {
        if (Healthbar != null)
        {
            Healthbar.SetHealth(Hitpoints, MaxHitpoints);
        }
    }
}