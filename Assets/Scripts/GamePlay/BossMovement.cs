using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _moveDuration = 2f;
    [SerializeField] private float _returnSpeed = 2f;

//     [SerializeField] private float _minSafeDistance = 2.5f; // boss keeps at least this far
// [SerializeField] private float _maxChaseRange = 6f;     // boss starts chasing if player farther than this


    private enum BossState { Patrol, Chase, Return }
    private BossState _currentState = BossState.Patrol;

    private Rigidbody2D _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Camera _camera;

    private Animator _animator;
    private Vector2 _moveDirection = Vector2.right;
    private float _directionTimer;
    private Vector3 _startPosition;
    private Vector2 _targetDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _directionTimer = _moveDuration;
        _startPosition = transform.position;
        _animator = GetComponent<Animator>();

        Debug.Log("<color=cyan>[Boss]</color> Initialized. Starting state: <b>Patrol</b>");
    }

    private void FixedUpdate()
    {
        // 🔄 Check awareness
        if (_playerAwarenessController.AwareOfPlayer && _currentState != BossState.Chase)
        {
            Debug.Log("<color=yellow>[Boss]</color> Player detected! Switching to <b>CHASE</b> mode 🧠");
            _currentState = BossState.Chase;
        }
        else if (!_playerAwarenessController.AwareOfPlayer && _currentState == BossState.Chase)
        {
            Debug.Log("<color=orange>[Boss]</color> Player lost... returning to <b>START POSITION</b> 🏠");
            _currentState = BossState.Return;
        }

        // 🚀 Run state logic
        switch (_currentState)
        {
            case BossState.Patrol:
                _animator.SetBool("bossWalk", false);
                HandlePatternMovement();
                break;
            case BossState.Chase:
                HandlePlayerChase();
               
                break;
            case BossState.Return:

                HandleReturnToStart();
                break;
        }
    }

    private void HandlePatternMovement()
    {
        _directionTimer -= Time.fixedDeltaTime;
        // Debug.Log("firstDirectionTimer: " + _directionTimer);
        if (_directionTimer <= 0f)
        {
            _moveDirection = -_moveDirection;
            FlipSprite();
            _directionTimer = _moveDuration;
            Debug.Log("<color=green>[Boss]</color> Flipping direction → Now moving " +
                      (_moveDirection.x > 0 ? "RIGHT ➡️" : "LEFT ⬅️"));
        }

        transform.Translate(_moveDirection * _speed * Time.fixedDeltaTime);
    }

    private void HandlePlayerChase()
    {
        _targetDirection = _playerAwarenessController.DirectionToPlayer;
        if (_targetDirection.x  < 0  && transform.localScale.x > 0)
        {
            FlipSprite();
        }
        else if (_targetDirection.x > 0 && transform.localScale.x < 0)
        {
            FlipSprite();
        }
        _animator.SetBool("bossWalk", true);
        transform.Translate(_targetDirection * _speed * Time.fixedDeltaTime);

        Debug.DrawLine(transform.position, transform.position + (Vector3)_targetDirection * 2f, Color.red);
        Debug.Log("<color=red>[Boss]</color> Chasing player → Direction: " + _targetDirection);
    }

// private void HandlePlayerChase()
// {
//     Vector2 bossPos = transform.position;
//     Vector2 playerPos = _playerAwarenessController._player.position;
//     float distanceToPlayer = Vector2.Distance(bossPos, playerPos);

//     _targetDirection = (playerPos - bossPos).normalized;

//     // 🧭 Face player
//     if (_targetDirection.x < 0 && transform.localScale.x > 0)
//         FlipSprite();
//     else if (_targetDirection.x > 0 && transform.localScale.x < 0)
//         FlipSprite();

//     _animator.SetBool("bossWalk", true);

//     // 🧍 Maintain safe distance
//     if (distanceToPlayer > _maxChaseRange)
//     {
//         // Move closer
//         transform.Translate(_targetDirection * _speed * Time.fixedDeltaTime);
//         Debug.Log("<color=red>[Boss]</color> Approaching player ➡️");
//     }
//     else if (distanceToPlayer < _minSafeDistance)
//     {
//         // Move away (back off)
//         transform.Translate(-_targetDirection * _speed * Time.fixedDeltaTime);
//         Debug.Log("<color=orange>[Boss]</color> Too close! Backing off ⬅️");
//     }
//     else
//     {
//         // Stay put (hover zone)
//         Debug.Log("<color=yellow>[Boss]</color> Holding position 👀");
//     }

//     Debug.DrawLine(bossPos, playerPos, Color.red);
// }

    private void HandleReturnToStart()
    {
        _animator.SetBool("bossIdle", true);

        Vector2 directionToStart = ((Vector2)_startPosition - (Vector2)transform.position).normalized;
        transform.Translate(directionToStart * _returnSpeed * Time.fixedDeltaTime);

        Debug.DrawLine(transform.position, _startPosition, Color.blue);
        Debug.Log("<color=blue>[Boss]</color> Returning to start position... Distance left: " +
                  Vector2.Distance(transform.position, _startPosition).ToString("F2"));

        if (Vector2.Distance(transform.position, _startPosition) < 0.1f)
        {
            transform.position = _startPosition;
            _currentState = BossState.Patrol;
            Debug.Log("<color=cyan>[Boss]</color> Returned home ✅ Switching back to <b>PATROL</b>");
        }
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
