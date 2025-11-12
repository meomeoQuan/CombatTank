using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Cần thiết cho List
using Assets.Scripts.DataController; // Để truy cập DataController
using Assets.Scripts.Models.Drones; // Để biết class Drone

public class DroneUI : MonoBehaviour
{
    [Tooltip("Tên của prefab 'DroneSlotUI' trong thư mục Resources.")]
    public string buttonPrefabName = "DroneSlotUI"; // Tên prefab của bạn

    [Tooltip("Tham chiếu đến GameObject cha (Parent) sẽ chứa các button.")]
    public Transform buttonsParent;

    /// <summary>
    /// Spawn các button và bơm dữ liệu.
    /// </summary>
    void Start()
    {
        // 1. Kiểm tra tham chiếu
        if (buttonsParent == null)
        {
            Debug.LogError("Chưa gán 'Buttons Parent'.");
            return;
        }

        // 2. Tải Prefab
        GameObject buttonPrefab = Resources.Load<GameObject>(buttonPrefabName);
        if (buttonPrefab == null)
        {
            Debug.LogError($"Không tìm thấy prefab tên là '{buttonPrefabName}' trong thư mục Resources.");
            return;
        }

        // Kiểm tra xem prefab có script DroneSlotUI không
        if (buttonPrefab.GetComponent<DroneSlotUI>() == null)
        {
            Debug.LogError($"Prefab '{buttonPrefabName}' đang thiếu script 'DroneSlotUI.cs'. Vui lòng thêm script đó vào prefab.");
            return;
        }

        // 3. Lấy Dữ liệu từ DataController
        // (Giả sử DataController.Initialize() đã chạy trước đó)
        List<Drone> allDrones = DataController.Drones;
        PlayerData playerData = DataController.Player;

        if (allDrones == null || playerData == null)
        {
            Debug.LogError("DataController.Drones hoặc DataController.Player chưa được khởi tạo!");
            return;
        }

        // 4. Spawn và thiết lập Parent dựa trên DỮ LIỆU
        // (Vòng lặp này sẽ chạy số lần bằng số lượng drone có trong DataController)
        foreach (Drone droneData in allDrones)
        {
            // Instantiate (Tạo bản sao) prefab
            GameObject newButtonGO = Instantiate(buttonPrefab);

            // Thiết lập GameObject cha (Parent)
            newButtonGO.transform.SetParent(buttonsParent, false);
            newButtonGO.name = $"DroneSlot_{droneData.Id}"; // Đặt tên cho dễ debug

            // Lấy script DroneSlotUI từ prefab vừa tạo
            DroneSlotUI slotScript = newButtonGO.GetComponent<DroneSlotUI>();

            // --- ĐÂY LÀ PHẦN QUAN TRỌNG NHẤT ---
            // Bơm (Inject) dữ liệu vào cho slot tự xử lý
            slotScript.Setup(droneData, playerData);

            // Gán sự kiện cho button
            // Chúng ta cần tạo một biến local (local copy) để lambda bắt đúng
            string droneID = droneData.Id;
            slotScript.slotButton.onClick.AddListener(() => OnDroneSlotClicked(droneID, slotScript));
        }
    }

    /// <summary>
    /// Hàm được gọi khi một button Drone được click.
    /// </summary>
    void OnDroneSlotClicked(string droneID, DroneSlotUI clickedSlot)
    {
        Debug.Log($"Button Drone '{droneID}' đã được nhấn!");

        // Từ đây, bạn có thể xử lý logic
        // Ví dụ:
        // if (!DataController.Player.IsDroneUnlocked(droneID))
        // {
        //     // Hiển thị popup Mua/Mở khóa
        //     ShopManager.Instance.ShowUnlockPopup(droneID);
        // }
        // else
        // {
        //     // Hiển thị popup chi tiết/nâng cấp
        //     UpgradeManager.Instance.ShowDroneDetails(droneID);
        // }
    }
}