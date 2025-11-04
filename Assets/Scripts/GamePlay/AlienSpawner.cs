using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private float _minimumSpawnTime;

    [SerializeField]
    private float _maximumSpawnTime;

    [SerializeField]
    private int _maxEnemies = 5; //  Số lượng enemy tối đa được spawn

    private int _spawnedCount = 0; //  Đếm số enemy đã spawn

    private float _timeUntilSpawn;

    void Awake()
    {
        SetTimeUntilSpawn();
    }

    void Update()
    {
        // Nếu đã spawn đủ 5 enemy thì ngừng lại
        if (_spawnedCount >= _maxEnemies)
        return;

        _timeUntilSpawn -= Time.deltaTime;

        if (_timeUntilSpawn <= 0)
        {
            Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
<<<<<<< HEAD
            _enemyPrefab.tag = "Enemy";
=======
>>>>>>> quan
            _spawnedCount++; //  Tăng số lượng enemy đã spawn
            SetTimeUntilSpawn();
        }
    }

    private void SetTimeUntilSpawn()
    {
        _timeUntilSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }
}