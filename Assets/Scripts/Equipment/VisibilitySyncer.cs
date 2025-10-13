using UnityEngine;
using TMPro; // Rất quan trọng để sử dụng TMP Text

public class VisibilitySyncer : MonoBehaviour
{
    [Tooltip("Kéo TMP Text B (hoặc GameObject chứa nó) vào đây.")]
    public GameObject targetObject;

    // Biến lưu trữ trạng thái hiển thị trước đó của đối tượng mục tiêu
    private bool wasActive;

    void Start()
    {
        // Kiểm tra xem đã gán đối tượng mục tiêu chưa
        if (targetObject == null)
        {
            Debug.LogError("Chưa gán 'Target Object' cho script VisibilitySyncer trên đối tượng " + gameObject.name);
            enabled = false; // Tắt script nếu không có mục tiêu
            return;
        }

        // Khởi tạo trạng thái ban đầu của Object A dựa trên TMP Text B
        wasActive = targetObject.activeInHierarchy;
        gameObject.SetActive(wasActive);
    }

    void Update()
    {
        // Kiểm tra trạng thái hiển thị hiện tại của TMP Text B
        bool isActive = targetObject.activeInHierarchy;

        // Chỉ xử lý nếu trạng thái thay đổi
        if (isActive != wasActive)
        {
            // Cập nhật trạng thái của Object A (đối tượng gắn script này)
            gameObject.SetActive(isActive);

            // Lưu trạng thái mới
            wasActive = isActive;
        }
    }
}