//using System.Collections.Generic;
//using UnityEngine;
//using Assets.Scripts.Models.Equipments;
//using Assets.Scripts.DataController;

//[System.Serializable]
//public class BulletType
//{
//    [Tooltip("Tên tag của loại đạn, ví dụ: Cannon1, Cannon2")]
//    public string nameTag;

//    [Tooltip("Prefab đạn (nếu để trống sẽ load từ Resources theo nameTag)")]
//    public GameObject prefab;

//    [Tooltip("Số lượng đạn tối đa trong Pool")]
//    public int poolSize = 20;

//    [HideInInspector] public Queue<GameObject> pool = new Queue<GameObject>();
//}

//public class BulletPoolManager : MonoBehaviour
//{
//    public static BulletPoolManager Instance;

//    [Header("Danh sách Bullet Types (tự build từ DataController hoặc chỉnh thủ công)")]
//    [SerializeField] private List<BulletType> bulletTypes = new List<BulletType>();

//    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

//    void Awake()
//    {
//        Instance = this;
//    }

//    private void AutoBuildFromDataController()
//    {
//        foreach (var eq in DataController.Equipments)
//        {
//            if (eq is Weapon weapon)
//            {
//                for (int i = 1; i <= 2; i++)
//                {
//                    string tag = weapon.Name + i;
//                    bulletTypes.Add(new BulletType
//                    {
//                        nameTag = tag,
//                        prefab = Resources.Load<GameObject>("Shell/" + tag), // load từ folder Shell
//                        poolSize = weapon.MaxAmmo
//                    });
//                }
//            }
//        }
//    }

//    private void MergeWithDataController()
//    {
//        foreach (var eq in DataController.Equipments)
//        {
//            if (eq is Weapon weapon)
//            {
//                for (int i = 1; i <= 2; i++)
//                {
//                    string tag = weapon.Name + i;

//                    // Nếu Inspector chưa có -> thêm mới
//                    if (!bulletTypes.Exists(bt => bt.nameTag == tag))
//                    {
//                        bulletTypes.Add(new BulletType
//                        {
//                            nameTag = tag,
//                            prefab = Resources.Load<GameObject>("Shell/" + tag), // load từ folder Shell
//                            poolSize = weapon.MaxAmmo
//                        });
//                    }
//                }
//            }
//        }
//    }

//    void Start()
//    {
//        if (bulletTypes.Count == 0)
//        {
//            AutoBuildFromDataController();
//        }
//        else
//        {
//            MergeWithDataController();
//        }

//        foreach (var type in bulletTypes)
//        {
//            if (type.prefab == null)
//            {
//                type.prefab = Resources.Load<GameObject>("Shell/" + type.nameTag);
//                if (type.prefab == null)
//                {
//                    Debug.LogWarning($"Prefab Shell/{type.nameTag} không tìm thấy!");
//                    continue;
//                }
//            }

//            type.pool = new Queue<GameObject>();
//            pools[type.nameTag] = type.pool;

//            for (int i = 0; i < type.poolSize; i++)
//            {
//                GameObject bullet = Instantiate(type.prefab, transform);
//                bullet.SetActive(false);
//                type.pool.Enqueue(bullet);
//            }

//            Debug.Log($"[BulletPool] Init {type.nameTag} với {type.poolSize} viên.");
//        }
//    }


//    public GameObject GetBullet(string bulletTag)
//    {
//        if (pools.ContainsKey(bulletTag))
//        {
//            Queue<GameObject> pool = pools[bulletTag];
//            if (pool.Count > 0)
//            {
//                GameObject bullet = pool.Dequeue();
//                bullet.SetActive(true);
//                return bullet;
//            }
//            else
//            {
//                Debug.LogWarning($"Bullet Pool {bulletTag} hết đạn!");
//                return null;
//            }
//        }
//        return null;
//    }

//    public void ReturnBullet(GameObject bullet, string bulletTag)
//    {
//        if (pools.ContainsKey(bulletTag))
//        {
//            bullet.SetActive(false);
//            pools[bulletTag].Enqueue(bullet);
//        }
//        else
//        {
//            Destroy(bullet);
//        }
//    }
//}
