using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private Transform _player;

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _moveDuration = 2f;
    [SerializeField] private float _returnSpeed = 2f;

    [Header("Boss Stats")]
    [SerializeField] private float _maxHP = 100f;
    private float _currentHP;
    private bool _hasRecovered = false;

    [Header("Boss Abilities")]
  [SerializeField] private GameObject _summonMonster;
[SerializeField] private Transform _summonPoint; // optional (can be null)
[SerializeField] private int _monsterCount = 5;
[SerializeField] private float _summonRadius = 2f;
      [SerializeField] private GameObject _fireballPrefab;   // Fireball prefab
          // Fire spawn position
    [SerializeField] private float _fireForce = 5f;

    private enum BossState { Patrol, Chase, Return, Summon, FireAttack, Recover, GetHit, Death }
    private BossState _currentState = BossState.Patrol;

    private Rigidbody2D _rigidbody;
    private PlayerAwarenessController _playerAwarenessController;
    private Animator _animator;
    private Vector2 _moveDirection = Vector2.right;
    private float _directionTimer;
    private Vector3 _startPosition;
    private Vector2 _targetDirection;

    // 🧠 Summon tracking
    private List<GameObject> _activeMinions = new List<GameObject>();
    private bool _canSummon = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwarenessController>();
        _directionTimer = _moveDuration;
        _startPosition = transform.position;
        _animator = GetComponent<Animator>();
        _currentHP = _maxHP;
    }

private void FixedUpdate()
{
    CheckHPLogic();
    CleanupDeadMinions();
        if (!_playerAwarenessController && _currentState != BossState.Chase)
        {
            SetState(BossState.Patrol);
        }
        
    switch (_currentState)
    {
        case BossState.Patrol:
            if (_playerAwarenessController.AwareOfPlayer)
                SetState(BossState.Chase);
            else
                HandlePatternMovement();
            break;

        case BossState.Chase:
            HandlePlayerChase();
            if (_playerAwarenessController.AwareOfPlayer)
                SetState(BossState.Summon);
            else
                SetState(BossState.Return);
            break;

        case BossState.Summon:
            if (_canSummon)
                StartCoroutine(HandleSummonState());
            break;

        case BossState.Return:
            HandleReturnToStart();
            break;

        case BossState.FireAttack:
                FireAttack();
          
            break;

        case BossState.Recover:
            Recover();
            break;

        case BossState.Death:
            Death();
            break;
    }

    // FireAttack trigger condition
    if (_activeMinions.Count == 1 && _currentState != BossState.FireAttack && !_isFiring)
        SetState(BossState.FireAttack);
}

private void SetState(BossState newState)
{
    if (_currentState == newState) return;
    Debug.Log($"<color=cyan>[Boss]</color> State changed: {_currentState} → <b>{newState}</b>");
    _currentState = newState;
}


    private void HandlePatternMovement()
    {
        _directionTimer -= Time.fixedDeltaTime;
        if (_directionTimer <= 0f)
        {
            _moveDirection = -_moveDirection;
            FlipSprite();
            _directionTimer = _moveDuration;
        }
        transform.Translate(_moveDirection * _speed * Time.fixedDeltaTime);
    }

    private void HandlePlayerChase()
    {
        _targetDirection = _playerAwarenessController.DirectionToPlayer;
        // Debug.Log($"<color=orange>[Boss]</color> Chasing player towards {_targetDirection}");
        if ((_targetDirection.x < 0 && transform.localScale.x > 0) ||
            (_targetDirection.x > 0 && transform.localScale.x < 0))
            FlipSprite();

        _animator.SetBool("IsBossWalk", true);
        transform.Translate(_targetDirection * _speed * Time.fixedDeltaTime);
    }

    private void HandleReturnToStart()
    {
        _animator.SetBool("IsBossIdle", true);
        Vector2 directionToStart = ((Vector2)_startPosition - (Vector2)transform.position).normalized;
        transform.Translate(directionToStart * _returnSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, _startPosition) < 0.1f)
        {
            transform.position = _startPosition;
            _currentState = BossState.Patrol;
        }
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ==========================================================
    // 🔥 BOSS ABILITIES BELOW
    // ==========================================================
private IEnumerator HandleSummonState()
{
    if (!_canSummon) yield break; // ❌ don't start again

    _canSummon = false;
    _animator.SetBool("IsBossSumon", true);

    Summon();

    // Wait for summon animation (non-looped, only once)
    yield return new WaitForSeconds(3f);

    _animator.SetBool("IsBossSumon", false);
    SetState(BossState.Patrol); // go back to patrol after summon
}

public void Summon()
{
    _animator.SetBool("IsBossSumon",true);
    Debug.Log("<color=purple>[Boss]</color> Summoning minions...");

    _canSummon = false;
    _activeMinions.Clear();

    int summonCount = _monsterCount;
    float radius = _summonRadius;

    // make sure we have a prefab
    if (_summonMonster == null)
    {
        Debug.LogWarning("⚠️ No summon monster prefab assigned!");
        return;
    }

    // pick the center point (boss or summon point)
    Vector3 center = _summonPoint != null ? _summonPoint.position : transform.position;

    for (int i = 0; i < summonCount; i++)
    {
        // calculate position around the boss in a circle
        float angle = i * Mathf.PI * 2f / summonCount;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        Vector3 spawnPos = center + offset;

        // spawn the monster
        GameObject minion = Instantiate(_summonMonster, spawnPos, Quaternion.identity);

        // assign player reference (auto)
  Enemy enemyScript = minion.GetComponent<Enemy>();
if (enemyScript != null)
{
    // if player reference not set manually, find it automatically
    if (_player == null)
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;

    enemyScript.player = _player;
}


        _activeMinions.Add(minion);
    }
}




   private bool _isFiring = false;

public void FireAttack()
{
    if (_isFiring) return;
    StartCoroutine(HandleFireAttack());
}

private IEnumerator HandleFireAttack()
{
    _isFiring = true;
    _animator.SetBool("IsBossFireAttack", true);
    Debug.Log("<color=orange>[Boss]</color> 🔥 Summoning fire trap...");

    yield return new WaitForSeconds(0.5f); // wait for animation to reach the moment boss hits the ground or casts

    SpawnFireTrap();

    yield return new WaitForSeconds(1.5f); // wait until animation ends
    _animator.SetBool("IsBossFireAttack", false);
    _isFiring = false;

    // switch back to normal state
    if (_playerAwarenessController.AwareOfPlayer)
        _currentState = BossState.Chase;
    else
        _currentState = BossState.Patrol;
}

public void SpawnFireTrap()
{
    if (_fireballPrefab == null)
    {
        Debug.LogWarning("⚠️ No fire prefab assigned!");
        return;
    }

    // define random spawn range (adjust as you want)
    float minX = -10f;
    float maxX = 10f;
    float minY = -3f;
    float maxY = 3f;

    // number of fire traps per summon
    int fireCount = Random.Range(2, 5); // 2~4 fires

    for (int i = 0; i < fireCount; i++)
    {
        Vector3 randomPos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        Instantiate(_fireballPrefab, randomPos, Quaternion.identity);
        Debug.Log($"<color=red>🔥 Fire trap spawned at {randomPos}</color>");
    }
}

    public void Recover()
    {
        _animator.SetTrigger("IsBossRecover");
        Debug.Log("<color=green>[Boss]</color> Recovering HP...");

        _currentHP = _maxHP * 0.8f;
        _hasRecovered = true;
        _currentState = BossState.Chase;
    }

    public void GetHit(float damage)
    {
        if (_currentState == BossState.Death) return;

        _currentHP -= damage;
        _animator.SetTrigger("IsBossGetHit");
        Debug.Log($"<color=yellow>[Boss]</color> Got hit! HP: {_currentHP}/{_maxHP}");

        if (_currentHP <= 0)
        {
            _currentState = BossState.Death;
            Death();
        }
    }

    public void Death()
    {
        _animator.SetTrigger("IsBossDeath");
        Debug.Log("<color=gray>[Boss]</color> Boss is dead 💀");
        Destroy(gameObject, 2f);
    }

    // ==========================================================
    // ⚙️ Logic Helpers
    // ==========================================================

    private void CheckHPLogic()
    {
        if (!_hasRecovered && _currentHP <= _maxHP * 0.5f)
        {
            _currentState = BossState.Recover;
        }
    }

    private void CleanupDeadMinions()
    {
        _activeMinions.RemoveAll(m => m == null);

        // All dead → can summon again
        if (_activeMinions.Count == 0)
        {
            _canSummon = true;
            Debug.Log("<color=cyan>[Boss]</color> All minions dead, ready to summon again.");
        }
    }

    private int CountDeadMinions()
    {
        int deadCount = 0;
        foreach (var minion in _activeMinions)
        {
            if (minion == null) deadCount++;
        }
        return deadCount;
    }
}