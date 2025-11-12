using UnityEngine;
using UnityEngine.UI; // Cần thiết cho các thành phần UI như Button

public class DroneUI : MonoBehaviour
{
    [Tooltip("Tên của prefab button trong thư mục Resources.")]
    public string buttonPrefabName = "DroneUIButton";

    [Tooltip("Số lượng button muốn spawn.")]
    public int numberOfButtons = 10;

    [Tooltip("Tham chiếu đến GameObject cha (Parent) sẽ chứa các button. <em>Nên là GameObject có HorizontalLayoutGroup.</em>")]
    public Transform buttonsParent;

    /// <summary>
    /// Spawn các button và thiết lập bố cục.
    /// </summary>
    void Start()
    {
        // 1. Kiểm tra tham chiếu
        if (buttonsParent == null)
        {
            Debug.LogError("Chưa gán 'Buttons Parent'. Vui lòng gán một GameObject UI (Canvas/Panel) có HorizontalLayoutGroup.");
            return;
        }

        // 2. Tải Prefab
        GameObject buttonPrefab = Resources.Load<GameObject>(buttonPrefabName);

        if (buttonPrefab == null)
        {
            Debug.LogError($"Không tìm thấy prefab tên là '{buttonPrefabName}' trong thư mục Resources.");
            Debug.Log("Vui lòng đảm bảo rằng bạn đã có một Prefab Button tại đường dẫn: Assets/Resources/DroneUIButton.prefab");
            return;
        }

        // 3. Spawn và thiết lập Parent
        for (int i = 0; i < numberOfButtons; i++)
        {
            // Instantiate (Tạo bản sao) prefab
            GameObject newButtonGO = Instantiate(buttonPrefab);

            // Thiết lập GameObject cha (Parent) để HorizontalLayoutGroup quản lý
            newButtonGO.transform.SetParent(buttonsParent, false);
            // 'false' giữ nguyên kích thước/xoay cục bộ, quan trọng cho UI

            // Tùy chỉnh (Tùy chọn): Đặt tên cho dễ quản lý và tìm kiếm
            newButtonGO.name = $"DroneButton_{i + 1}";

            // Tùy chỉnh (Tùy chọn): Thay đổi chữ trên button
            Button buttonComponent = newButtonGO.GetComponent<Button>();
            if (buttonComponent != null)
            {
                // Tìm Text component con (ví dụ: trên Text (Legacy) hoặc TextMeshPro)
                Text buttonText = newButtonGO.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = $"Button {i + 1}";
                }

                // Gán sự kiện cho button (Ví dụ: in ra console khi click)
                buttonComponent.onClick.AddListener(() => OnButtonClick(i + 1));
            }
        }
    }

    /// <summary>
    /// Hàm được gọi khi một button được click.
    /// </summary>
    /// <param name="buttonID">ID của button được click.</param>
    void OnButtonClick(int buttonID)
    {
        Debug.Log($"Button Drone số {buttonID} đã được nhấn!");
    }

    // Không cần Update() cho chức năng này
    void Update()
    {

    }
}