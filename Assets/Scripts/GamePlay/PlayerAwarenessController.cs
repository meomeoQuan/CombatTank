using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool AwareOfPlayer { get; private set; } //enemy nhận thức xe tank 
    public Vector2 DirectionToPlayer { get; private set; } //enemy biết hướng xe tank
    [SerializeField]
    private float _playerAwarenessDistance; //khoảng cách enemy nhận thức xe tank
    private Transform _player; //cần để biết xe tank ở đâu 

    //dò chuyển động của xe tank
    private void Awake()
    {
        //tìm instance đầu tiên của component PlayerMovement trong scene và lấy Transform của nó để biết vị trí player.
        _player = Object.FindFirstObjectByType<PlayerMovement>().transform; 
    }

    // Update is called once per frame

    //kiểm tra xe tank có trong phạm vi không
    void Update() 
    {
        Vector2 enemyToPlayerVector = _player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;
        if(enemyToPlayerVector.magnitude <= _playerAwarenessDistance) //magnitude là độ dài khoảng cách 
        {
            AwareOfPlayer = true;
        }
        else
        {
            AwareOfPlayer = false;
        }
    }
}
