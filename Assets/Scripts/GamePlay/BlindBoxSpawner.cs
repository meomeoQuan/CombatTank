using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlindBoxSpawner : MonoBehaviour
{
    public GameObject blindBoxPrefab; // Kéo Prefab Blind Box vào đây từ Inspector
    public float spawnInterval = 20f; // Khoảng thời gian giữa các lần spawn (20 giây)
    public List<Vector2> spawnPoints; // Danh sách các vị trí có thể spawn blind box

    private GameObject currentBlindBoxInstance; // Giữ tham chiếu đến blind box hiện tại
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Bắt đầu gọi hàm SpawnBlindBox sau 0 giây, và lặp lại mỗi `spawnInterval` giây
        InvokeRepeating("SpawnBlindBox", 20f, spawnInterval);
    }

    void SpawnBlindBox()
    {
        // 1. Kiểm tra và hủy blind box cũ nếu có
        if (currentBlindBoxInstance != null)
        {
            Destroy(currentBlindBoxInstance);
            currentBlindBoxInstance = null; // Đảm bảo không còn tham chiếu
        }

        // 2. Chọn một vị trí spawn ngẫu nhiên
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("BlindBoxSpawner: No spawn points defined! Spawning at spawner's position.");
            currentBlindBoxInstance = Instantiate(blindBoxPrefab, transform.position, Quaternion.identity);
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count); // Lấy index ngẫu nhiên
        Vector2 randomSpawnPoint = spawnPoints[randomIndex];

        // 3. Sinh ra blind box mới tại vị trí đã chọn
        currentBlindBoxInstance = Instantiate(blindBoxPrefab, randomSpawnPoint, Quaternion.identity);
        Debug.Log($"Spawned a new BlindBox at {randomSpawnPoint}");
    }

    // (Tùy chọn) Vẽ các điểm spawn trong Scene View để dễ nhìn
    void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.cyan; // Màu xanh lam cho điểm spawn
            foreach (Vector2 point in spawnPoints)
            {
                Gizmos.DrawWireSphere(point, 0.5f); // Vẽ hình cầu tại mỗi điểm spawn
            }
        }
    }
}



// Quản lý nhiều blind box cùng lúc: Nếu bạn muốn có nhiều blind box cùng tồn tại trên màn hình, thay vì hủy cái cũ, bạn sẽ cần sửa đổi logic trong BlindBoxSpawner để nó chỉ spawn thêm mà không hủy cái cũ.
// Object Pooling: Đối với các game có nhiều đối tượng được tạo và hủy liên tục (như đạn, blind box), việc sử dụng Object Pooling là một kỹ thuật tối ưu hóa hiệu suất rất tốt. Thay vì Instantiate và Destroy, bạn sẽ "tái sử dụng" các đối tượng đã có sẵn. Đây là một bước nâng cao hơn nếu bạn gặp vấn đề về hiệu suất sau này.
// Vị trí Spawn động: Bạn có thể tạo các vùng spawn thay vì các điểm cố định, và chọn một vị trí ngẫu nhiên trong vùng đó.