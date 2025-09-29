using System.Collections.Generic;
using UnityEngine;

// Cấu trúc để dễ dàng quản lý các loại Pool khác nhau
[System.Serializable]
public class BulletType
{
    public string nameTag; // Ví dụ: "Shell1", "Shell2"
    public GameObject prefab;
    public int poolSize = 20;
    [HideInInspector] public Queue<GameObject> pool = new Queue<GameObject>();
}

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance;

    public List<BulletType> bulletTypes;
    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        foreach (var type in bulletTypes)
        {
            // Khởi tạo Queue và Dictionary
            type.pool = new Queue<GameObject>();
            pools.Add(type.nameTag, type.pool);

            // Tạo sẵn các viên đạn
            for (int i = 0; i < type.poolSize; i++)
            {
                GameObject bullet = Instantiate(type.prefab, transform);
                bullet.SetActive(false);
                type.pool.Enqueue(bullet);
            }
        }
    }

    // Lấy đạn: Truyền Tag của loại đạn muốn lấy
    public GameObject GetBullet(string bulletTag)
    {
        if (pools.ContainsKey(bulletTag))
        {
            Queue<GameObject> pool = pools[bulletTag];
            if (pool.Count > 0)
            {
                GameObject bullet = pool.Dequeue();
                bullet.SetActive(true);
                return bullet;
            }
            else
            {
                // Tùy chọn: Tăng trưởng Pool bằng cách tạo mới
                //BulletType type = bulletTypes.Find(t => t.nameTag == bulletTag);
                //if (type != null)
                //{
                //    GameObject bullet = Instantiate(type.prefab, transform);
                //    return bullet;
                //}
                Debug.LogWarning("Bullet Pool hết đạn! Không thể bắn.");
                return null; // Không tạo mới, trả về null nếu hết đạn trong Pool
            }
        }
        return null; // Không tìm thấy loại đạn
    }

    // Trả đạn về Pool: Cần biết nó thuộc loại nào để trả đúng Pool
    public void ReturnBullet(GameObject bullet, string bulletTag)
    {
        if (pools.ContainsKey(bulletTag))
        {
            bullet.SetActive(false);
            pools[bulletTag].Enqueue(bullet);
        }
        else
        {
            // Đạn không thuộc Pool nào đã định, nên hủy bỏ nó
            Destroy(bullet);
        }
    }

}