using UnityEngine;

public class SpawnerCollectableDrop : MonoBehaviour
{
    [SerializeField] private float _chanceOfCollectableDrop;
    private CollectableSpawner _collectableSpawner;
    private SpawnerHealthController _healthController;

    private void Awake()
    {
        _collectableSpawner = FindFirstObjectByType<CollectableSpawner>();
    }

    private void Start()
    {
        _healthController = GetComponent<SpawnerHealthController>();
        if (_healthController != null)
        {
            _healthController.OnDied.AddListener(OnSpawnerDied);
        }
    }

    private void OnSpawnerDied()
    {
        float random = Random.Range(0f, 1f);
        if (_chanceOfCollectableDrop >= random && _collectableSpawner != null)
        {
            _collectableSpawner.SpawnCollectable(transform.position);
        }
    }
    public void RandomlyDropCollectable()
    {
        Debug.Log($"[Drop] OnDied được gọi tại {name}");
        float random = Random.Range(0f, 1f);
        Debug.Log($"[Drop] Random={random}, Chance={_chanceOfCollectableDrop}");

        if (_chanceOfCollectableDrop >= random)
        {
            if (_collectableSpawner != null)
            {
                Debug.Log("[Drop] Spawning collectable at " + transform.position);
                _collectableSpawner.SpawnCollectable(transform.position);
            }
            else
            {
                Debug.LogWarning("[Drop] Không tìm thấy CollectableSpawner trong scene!");
            }
        }
    }
}
