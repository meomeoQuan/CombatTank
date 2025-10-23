using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

 public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField] 
    private float _playerAwarenessDistance;

    [SerializeField] 
    public Transform _player; // now assignable in Inspector 💪

    private void Awake()
    {
        // if not manually assigned, auto-find it
        if (_player == null)
        {
            var playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
            var singlePlayerMovement = Object.FindFirstObjectByType<SinglePlayerMovement>();

            _player = playerMovement != null
                ? playerMovement.transform
                : singlePlayerMovement?.transform;
        }
    }
    // Update is called once per frame

    //kiểm tra xe tank có trong phạm vi không
    void Update() 
    {
        Vector2 enemyToPlayerVector = _player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;
        if(enemyToPlayerVector.magnitude <= _playerAwarenessDistance)
        {
            AwareOfPlayer = true;
        }
        else
        {
            AwareOfPlayer = false;
        }
    }
}
