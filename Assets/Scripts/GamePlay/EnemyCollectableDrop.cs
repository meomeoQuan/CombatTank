using UnityEngine;

public class EnemyCollectableDrop : MonoBehaviour
{
    [SerializeField]
    private float _chanceOfCollectableDrop; //cơ hội phần trăm từ 0 tới 1
    //giá trị càng cao thì kích hoạt vật phẩm càng cao 

    private CollectableSpawner _collectableSpawner;
    private HealthController _healthController;

    private void Awake()
    {
        //tìm ra vật phẩm sưu tầm và gán vào battle
        _collectableSpawner = FindFirstObjectByType<CollectableSpawner>();
    }

    private void Start()
    {
        var health = GetComponent<HealthController>();
        if (health != null)
        {
            health.OnDied.AddListener(OnEnemyDied);
            Debug.Log($"[Drop] Đăng ký sự kiện OnDied cho {name}");
        }
    }

    private void OnEnemyDied()
    {
        Debug.Log($"[Drop] Enemy {name} chết → drop vật phẩm");
        RandomlyDropCollectable();
        Destroy(gameObject);
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
