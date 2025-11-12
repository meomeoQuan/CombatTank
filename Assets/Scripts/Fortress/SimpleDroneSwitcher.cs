using UnityEngine;

public class SimpleDroneSwitcher : MonoBehaviour
{
    [Header("Drone Setup")]
    public Transform dronePoint;           // Vị trí spawn drone quanh player
    public GameObject[] dronePrefabs;      // Danh sách prefab drone
    private GameObject currentDrone;       // Drone hiện tại
    private int currentIndex = 0;          // Drone đang hiển thị

    void Start()
    {
        if (dronePrefabs.Length > 0)
            SpawnDrone(currentIndex); // Spawn drone đầu tiên
        else
            Debug.LogWarning("⚠️ DronePrefabs đang rỗng! Vui lòng gán prefab.");
    }

    void Update()
    {
        if (dronePrefabs.Length == 0) return; // ❌ Không có drone thì thoát

        // ✅ Nhấn 1,2,3 để đổi drone
        if (Input.GetKeyDown(KeyCode.Alpha1)) SpawnDroneAtIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnDroneAtIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SpawnDroneAtIndex(2);
    }

    void SpawnDroneAtIndex(int index)
    {
        if (index < 0 || index >= dronePrefabs.Length) return;

        currentIndex = index;
        SpawnDrone(currentIndex);
    }

    void SpawnDrone(int index)
    {
        // Xóa drone cũ
        if (currentDrone != null)
            Destroy(currentDrone);

        // Spawn drone mới
        currentDrone = Instantiate(dronePrefabs[index], dronePoint.position, Quaternion.identity);

        // Gán player cho drone
        Drone droneScript = currentDrone.GetComponent<Drone>();
        if (droneScript != null)
        {
            droneScript.player = this.transform;           // player là tank hiện tại
            droneScript.orbitAngle = Random.Range(0f, 360f); // tránh chồng nhau
        }

        Debug.Log($"🚁 Đã đổi sang drone: {dronePrefabs[index].name}");
    }
}
