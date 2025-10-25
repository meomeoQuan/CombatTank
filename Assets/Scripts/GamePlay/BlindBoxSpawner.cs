using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlindBoxSpawner : MonoBehaviour
{
    public GameObject blindBoxPrefab; // Kéo Prefab vào đây từ Inspector
    public float spawnInterval = 20f; // Khoảng thời gian giữa các lần spawn (giây)
    public List<Vector2> spawnPoints; // Danh sách các vị trí có thể spawn blind box

    private GameObject currentBlindBoxInstance; // Giữ tham chiếu đến blind box hiện tại
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Bắt đầu gọi hàm SpawnBlindBox sau 20 giây, và lặp lại mỗi `spawnInterval` giây
        InvokeRepeating("SpawnBlindBox", 20f, spawnInterval);
    }

    void SpawnBlindBox()
    {
        // Kiểm tra và hủy blind box cũ 
        if (currentBlindBoxInstance != null)
        {
            Destroy(currentBlindBoxInstance);
            currentBlindBoxInstance = null; // Đảm bảo không còn tham chiếu
        }

        // Chọn một vị trí spawn ngẫu nhiên nếu spawnPoints kh có
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("BlindBoxSpawner: No spawn points defined! Spawning at spawner's position.");
            currentBlindBoxInstance = Instantiate(blindBoxPrefab, transform.position, Quaternion.identity);
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Vector2 randomSpawnPoint = spawnPoints[randomIndex];

        currentBlindBoxInstance = Instantiate(blindBoxPrefab, randomSpawnPoint, Quaternion.identity);
        Debug.Log($"Spawned a new BlindBox at {randomSpawnPoint}");
    }

    // (Tùy chọn) Vẽ các điểm spawn trong Scene View để dễ nhìn
    void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.blue; // Màu xanh lam cho điểm spawn
            foreach (Vector2 point in spawnPoints)
            {
                Gizmos.DrawWireSphere(point, 3f); // Vẽ hình cầu tại mỗi điểm spawn
            }
        }
    }
}



// Quản lý nhiều blind box cùng lúc: Nếu bạn muốn có nhiều blind box cùng tồn tại trên màn hình, thay vì hủy cái cũ, bạn sẽ cần sửa đổi logic trong BlindBoxSpawner để nó chỉ spawn thêm mà không hủy cái cũ.
// Object Pooling: Đối với các game có nhiều đối tượng được tạo và hủy liên tục (như đạn, blind box), việc sử dụng Object Pooling là một kỹ thuật tối ưu hóa hiệu suất rất tốt. Thay vì Instantiate và Destroy, bạn sẽ "tái sử dụng" các đối tượng đã có sẵn. Đây là một bước nâng cao hơn nếu bạn gặp vấn đề về hiệu suất sau này.
// Vị trí Spawn động: Bạn có thể tạo các vùng spawn thay vì các điểm cố định, và chọn một vị trí ngẫu nhiên trong vùng đó.