// Tên file: DroneSlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Models.Drones;
using Assets.Scripts.DataController;
using TMPro; // Sử dụng TextMeshPro (thay bằng 'Text' nếu bạn dùng Legacy Text)

public class DroneSlotUI : MonoBehaviour
{
    [Header("Tham chiếu Component (Kéo thả từ Prefab)")]
    [Tooltip("Hình ảnh Icon của Drone")]
    public Image droneIcon;

    [Tooltip("Text để hiển thị số lượng (ví dụ: 'x5')")]
    public TextMeshProUGUI quantityText; // Đổi thành 'public Text quantityText;' nếu dùng Legacy

    // === THAY ĐỔI Ở ĐÂY ===
    [Tooltip("Tham chiếu đến chính component Image của ổ khóa")]
    public Image lockOverlay; // Đã đổi từ GameObject sang Image

    [Tooltip("Button của chính Slot này")]
    public Button slotButton;

    private Drone associatedDrone;
    private PlayerData currentPlayer;

    /// <summary>
    /// Hàm chính để 'bơm' dữ liệu vào Slot này.
    /// </summary>
    public void Setup(Drone droneData, PlayerData playerData)
    {
        associatedDrone = droneData;
        currentPlayer = playerData;

        // 1. Cập nhật thông tin cơ bản
        droneIcon.sprite = droneData.Icon;

        // 2. Kiểm tra trạng thái Mở khóa / Số lượng
        bool isUnlocked = playerData.IsDroneUnlocked(droneData.Id);
        int quantity = playerData.GetDroneQuantity(droneData.Id);

        if (isUnlocked)
        {
            // --- TRẠNG THÁI: ĐÃ MỞ KHÓA ---

            // === THAY ĐỔI Ở ĐÂY ===
            // Tắt GameObject mà component Image này gắn vào
            lockOverlay.gameObject.SetActive(false);

            // Hiển thị số lượng
            quantityText.gameObject.SetActive(true);
            quantityText.text = $"x{quantity}";
        }
        else
        {
            // --- TRẠNG THÁI: CHƯA MỞ KHÓA ---

            // === THAY ĐỔI Ở ĐÂY ===
            // Bật GameObject mà component Image này gắn vào
            lockOverlay.gameObject.SetActive(true);

            quantityText.gameObject.SetActive(false); // Ẩn số lượng
        }
    }
}