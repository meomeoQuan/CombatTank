using Assets.Scripts.DataController;
using Assets.Scripts.Models.Equipments;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class EquipmentSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- Các biến SerializeField ---
    [SerializeField] private Image icon;                  // Hình ảnh trang bị
    [SerializeField] private TMP_Text nameText;           // Tên trang bị
    [SerializeField] private TMP_Text indicatorLeft;      // Hiện "1" nếu được trang bị cho nhân vật 1
    [SerializeField] private TMP_Text indicatorRight;     // Hiện "2" nếu được trang bị cho nhân vật 2

    // --- Biến nội bộ ---
    private EquipmentBase equipmentData;
    private System.Action<EquipmentBase> onClickCallback;
    private InventoryUI inventoryUI;                     // Tham chiếu tới InventoryUI
    private Button btn;
    private Coroutine hoverCoroutine;                    // 🔥 coroutine hiển thị panel sau delay

    // --- Lifecycle Methods ---
    private void Awake()
    {
        btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (btn != null)
            btn.onClick.RemoveListener(OnClick);
    }

    // --- Public Methods ---

    /// <summary>
    /// Thiết lập dữ liệu và callback khi tạo slot
    /// </summary>
    public void Setup(EquipmentBase equipment, System.Action<EquipmentBase> onClick, InventoryUI parentUI = null)
    {
        equipmentData = equipment;
        onClickCallback = onClick;
        inventoryUI = parentUI;

        if (equipment == null)
        {
            // Thiết lập trạng thái rỗng
            if (icon != null)
            {
                icon.sprite = null;
                var c = icon.color;
                c.a = 0f;
                icon.color = c;
            }

            if (nameText != null)
                nameText.text = "";

            indicatorLeft?.gameObject.SetActive(false);
            indicatorRight?.gameObject.SetActive(false);
            return;
        }

        // Gán icon + tên
        if (icon != null)
        {
            icon.sprite = equipment.Icon;
            icon.color = Color.white;
        }

        if (nameText != null)
            nameText.text = equipment.Name;

        UpdateIndicators(equipment);
    }

    /// <summary>
    /// Xử lý khi bấm vào slot
    /// </summary>
    public void OnClick()
    {
        onClickCallback?.Invoke(equipmentData);
    }

    /// <summary>
    /// Trả về dữ liệu trang bị
    /// </summary>
    public EquipmentBase GetEquipmentData()
    {
        return equipmentData;
    }

    // --- Interface Implementations ---

    /// <summary>
    /// Khi chuột di vào slot — hiển thị thông tin chi tiết (sau 0.5s)
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equipmentData != null)
        {
            // 🔥 bắt đầu Coroutine đợi 0.5 giây
            hoverCoroutine = StartCoroutine(ShowInfoAfterDelay(0.5f));
        }
    }

    /// <summary>
    /// Khi chuột rời khỏi slot — hủy coroutine & ẩn panel thông tin
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }

        // Yêu cầu InventoryUI ẩn panel thông tin
        inventoryUI?.HideEquipmentInfo();
    }

    // --- Private/Helper Methods ---

    /// <summary>
    /// Cập nhật hiển thị “1” hoặc “2” nếu trang bị được sử dụng bởi nhân vật tương ứng
    /// </summary>
    private void UpdateIndicators(EquipmentBase equipment)
    {
        if (indicatorLeft == null || indicatorRight == null)
            return;

        // Giả định DataController.Characters là một danh sách các nhân vật
        if (DataController.Characters == null || DataController.Characters.Count < 2)
        {
            indicatorLeft.gameObject.SetActive(false);
            indicatorRight.gameObject.SetActive(false);
            return;
        }

        var char1 = DataController.Characters[0];
        var char2 = DataController.Characters[1];

        // Kiểm tra xem trang bị có được dùng bởi nhân vật 1 hoặc 2 không
        bool equippedBy1 = char1.EquippedItems.Values.Contains(equipment);
        bool equippedBy2 = char2.EquippedItems.Values.Contains(equipment);

        indicatorLeft.gameObject.SetActive(equippedBy1);
        indicatorRight.gameObject.SetActive(equippedBy2);

        if (equippedBy1) indicatorLeft.text = "1";
        if (equippedBy2) indicatorRight.text = "2";
    }

    /// <summary>
    /// Coroutine: đợi 0.5 giây rồi hiện panel info
    /// </summary>
    private IEnumerator ShowInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Yêu cầu InventoryUI hiển thị panel thông tin tại vị trí của slot
        inventoryUI?.ShowEquipmentInfo(equipmentData, transform.position);
    }
}