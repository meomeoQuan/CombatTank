using Assets.Scripts.DataController;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Equipments;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections; // Đảm bảo có using System.Collections cho IEnumerator

public class InventoryUI : MonoBehaviour
{
    #region 🧩 Inspector Fields

    [Header("UI References")]
    [SerializeField] private Transform contentParent;   // Nơi chứa các slot trang bị
    [SerializeField] private GameObject slotPrefab;     // Prefab của từng ô slot
    [SerializeField] private EquippedUIManager equippedUIManager; // Quản lý giao diện trang bị hiện tại

    [Header("Confirm Button")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text confirmButtonText;

    [Header("Equipment Info Panel")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text infoName;
    [SerializeField] private TMP_Text infoDescription;
    [SerializeField] private Image infoIcon;

    #endregion

    #region 💾 Private Fields

    private EquipmentBase selectedEquipment;  // Item đang được chọn
    private Character selectedCharacter;      // Nhân vật đang thao tác
    private bool isEquippedSelected;          // Trạng thái item đang được trang bị hay không

    private float lastScrollY;                     // Lưu vị trí cuộn trước đó
    private const float SCROLL_HIDE_THRESHOLD = 25f; // Nếu lệch quá nhiều thì ẩn nút xác nhận
    private bool suppressScrollCheck = false;       // Tạm tắt kiểm tra scroll
    private ScrollRect scroll;                      // ScrollRect chính trong Inventory

    private List<EquipmentBase> equipments;         // Danh sách tất cả trang bị
    // Thuộc tính chỉ đọc để lấy nhân vật hiện tại một cách an toàn
    private Character currentCharacter => equippedUIManager != null ? equippedUIManager.GetCurrentCharacter() : null;

    #endregion

    #region 🚀 Unity Lifecycle

    private void Start()
    {
        // Ẩn nút xác nhận khi khởi động
        if (confirmButton != null)
        {
            confirmButton.gameObject.SetActive(false);
            // Gán listener cho nút xác nhận
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        equipments = DataController.Equipments;

        if (equippedUIManager == null)
            Debug.LogWarning("[InventoryUI] ⚠ equippedUIManager chưa được gán trong Inspector.");

        // Theo dõi sự kiện cuộn để ẩn nút xác nhận khi người dùng cuộn mạnh
        scroll = GetComponentInChildren<ScrollRect>();
        if (scroll != null)
        {
            // Lấy ScrollRect của chính contentParent nếu nó là ScrollRect
            if (scroll == null && contentParent.parent != null)
                scroll = contentParent.parent.GetComponent<ScrollRect>();

            if (scroll != null)
            {
                scroll.onValueChanged.AddListener((_) =>
                {
                    if (suppressScrollCheck) return;

                    float currentY = scroll.content.anchoredPosition.y;
                    if (confirmButton != null && confirmButton.gameObject.activeSelf &&
                        Mathf.Abs(currentY - lastScrollY) > SCROLL_HIDE_THRESHOLD)
                    {
                        HideConfirmButton();
                    }
                });
            }
        }
    }

    #endregion

    #region 🧱 Populate & Refresh UI

    /// <summary>
    /// Hiển thị chỉ những Equipment có cùng Type truyền vào.
    /// Nếu character = null thì sẽ lấy currentCharacter từ manager (nếu có).
    /// </summary>
    public void FilterByType(System.Type type, Character character)
    {
        // an toàn lấy character thực tế
        var cc = character ?? (equippedUIManager != null ? equippedUIManager.GetCurrentCharacter() : null);

        // kiểm tra thiết lập UI
        if (contentParent == null || slotPrefab == null)
        {
            Debug.LogError("[InventoryUI] FilterByType: contentParent hoặc slotPrefab chưa gán!");
            return;
        }

        // xoá tất cả slot hiện tại
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        // đảm bảo danh sách equipments có dữ liệu
        equipments = DataController.Equipments;
        if (equipments == null || equipments.Count == 0)
        {
            Debug.LogWarning("[InventoryUI] FilterByType: không có equipment nào trong DataController.");
            return;
        }

        // tạo các slot chỉ với loại trùng với "type"
        foreach (var eq in equipments)
        {
            if (eq == null) continue;

            // dùng exact type match. Nếu bạn muốn include subclasses dùng: type.IsAssignableFrom(eq.GetType())
            if (eq.GetType() != type)
                continue;

            var go = Instantiate(slotPrefab, contentParent);
            var slotUI = go.GetComponent<EquipmentSlotUI>();
            if (slotUI == null)
            {
                Debug.LogWarning("[InventoryUI] FilterByType: slotPrefab thiếu component EquipmentSlotUI!");
                Destroy(go);
                continue;
            }

            // truyền InventoryUI (this) để slot có thể gọi ShowEquipmentInfo
            slotUI.Setup(eq, (e) => OnSlotClicked(e, cc), this);
        }
    }

    /// <summary>
    /// Làm mới toàn bộ UI (gọi khi đổi nhân vật hoặc cập nhật dữ liệu)
    /// </summary>
    public void RefreshUI(Character character)
    {
        if (contentParent == null || slotPrefab == null)
        {
            Debug.LogError("[InventoryUI] ❌ contentParent hoặc slotPrefab chưa được gán trong Inspector!");
            return;
        }

        PopulateInventory(character);
    }

    /// <summary>
    /// Tạo danh sách tất cả các slot item trong Inventory
    /// </summary>
    public void PopulateInventory(Character character)
    {
        var cc = character ?? (equippedUIManager != null ? equippedUIManager.GetCurrentCharacter() : null);

        // Xóa các slot cũ
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        equipments = DataController.Equipments;

        foreach (var eq in equipments)
        {
            var go = Instantiate(slotPrefab, contentParent);
            var slotUI = go.GetComponent<EquipmentSlotUI>();
            if (slotUI == null)
            {
                Debug.LogWarning("[InventoryUI] ⚠ slotPrefab thiếu component EquipmentSlotUI!");
                continue;
            }

            slotUI.Setup(eq, (e) => OnSlotClicked(e, cc), this);
        }
    }

    #endregion

    #region 🖱️ Slot Click Handling

    /// <summary>
    /// Khi người dùng bấm vào 1 slot
    /// </summary>
    private void OnSlotClicked(EquipmentBase equipment, Character character)
    {
        var cc = character ?? currentCharacter;
        if (cc == null)
        {
            Debug.LogWarning($"[InventoryUI] ⚠ Không có character để equip/unequip {equipment?.Name}.");
            return;
        }

        // Bấm lại cùng item → toggle tắt nút
        if (selectedEquipment == equipment && confirmButton != null && confirmButton.gameObject.activeSelf)
        {
            confirmButton.gameObject.SetActive(false);
            selectedEquipment = null;
            return;
        }

        selectedEquipment = equipment;
        selectedCharacter = cc;
        isEquippedSelected = cc.EquippedItems.Values.Contains(equipment);

        // Hiển thị nút xác nhận
        if (confirmButton != null && confirmButtonText != null)
        {
            confirmButtonText.text = isEquippedSelected ? "UNEQUIP" : "EQUIP";
            confirmButton.gameObject.SetActive(true);

            // Ghi lại vị trí scroll để theo dõi và tránh ẩn nút ngay lập tức
            if (scroll != null)
            {
                lastScrollY = scroll.content.anchoredPosition.y;
                StartCoroutine(TemporarilySuppressScroll());
            }
        }

        // Căn vị trí nút xác nhận theo vị trí slot
        var slotRect = EventSystem.current.currentSelectedGameObject?.GetComponent<RectTransform>();
        if (slotRect != null && confirmButton != null)
        {
            var btnRect = confirmButton.GetComponent<RectTransform>();
            var canvasRect = btnRect.parent as RectTransform;

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, slotRect.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos);

            // Căn vị trí lên trên hoặc xuống dưới slot, dựa vào vị trí Y của slot
            // Sử dụng -200 làm ngưỡng: nếu slot nằm thấp (localPos.y < -200), đặt nút lên trên (145), ngược lại đặt xuống dưới (-145).
            btnRect.anchoredPosition = (localPos.y < -200)
                ? localPos + new Vector2(0, 145)
                : localPos + new Vector2(0, -145);
        }
    }

    /// <summary>
    /// Khi người dùng bấm nút EQUIP/UNEQUIP
    /// </summary>
    public void OnConfirmButtonClicked()



    {
        if (selectedEquipment == null || selectedCharacter == null)
        {
            Debug.LogWarning("[InventoryUI] ⚠ Không có item hoặc character được chọn để xác nhận.");
            return;
        }

        if (isEquippedSelected)
        {
            Debug.Log($"[InventoryUI] 🧤 Unequip {selectedEquipment.Name}");
            selectedCharacter.Unequip(selectedEquipment);
        }
        else
        {
            Debug.Log($"[InventoryUI] 🗡️ Equip {selectedEquipment.Name}");
            selectedCharacter.Equip(selectedEquipment);
        }

        confirmButton.gameObject.SetActive(false);
        selectedEquipment = null;
        selectedEquipment = null;

        // Làm mới UI
        equippedUIManager?.RefreshCurrentCharacter();
        RefreshUI(currentCharacter);
    }

    public void HideConfirmButton()
    {
        if (confirmButton != null && confirmButton.gameObject.activeSelf)
        {
            confirmButton.gameObject.SetActive(false);
            selectedEquipment = null;
        }
    }

    private System.Collections.IEnumerator TemporarilySuppressScroll()
    {
        suppressScrollCheck = true;
        yield return new WaitForSeconds(0.2f);
        suppressScrollCheck = false;
    }

    #endregion

    #region 🔍 Info Panel (Hover Tooltip)

    /// <summary>
    /// Hiển thị panel thông tin chi tiết khi hover vào item
    /// </summary>
    public void ShowEquipmentInfo(EquipmentBase equipment, Vector3 slotPos)
    {
        if (equipment == null || infoPanel == null || infoName == null || infoIcon == null || infoDescription == null) return;

        infoPanel.SetActive(true);
        infoName.text = equipment.Name;
        infoIcon.sprite = equipment.Icon;

        // 🧠 Xây dựng mô tả theo từng loại trang bị
        string desc = equipment switch
        {
            Armor armor => BuildArmorInfo(armor),
            Necklace necklace => BuildNecklaceInfo(necklace),
            Weapon weapon => BuildWeaponInfo(weapon),
            Shoes shoes => BuildShoesInfo(shoes),
            Pants pants => BuildPantsInfo(pants),
            Hat hat => BuildHatInfo(hat),
            _ => "<b>Type:</b> Common Equipment\n(No special stats)"
        };

        infoDescription.text = desc;

        // 🧩 Căn vị trí hiển thị panel
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, slotPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            infoPanel.transform.parent as RectTransform,
            screenPos,
            null,
            out Vector2 localPos
        );

        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        if (localPos.x < -350 && localPos.y > 350)
            panelRect.anchoredPosition = localPos + new Vector2(300, -150);
        else if (localPos.x < -350)
            panelRect.anchoredPosition = localPos + new Vector2(300, 0);
        else if (localPos.y < -350)
            panelRect.anchoredPosition = localPos + new Vector2(-300, 150);
        else if (localPos.y > 350)
            panelRect.anchoredPosition = localPos + new Vector2(-300, -150);
        else
            panelRect.anchoredPosition = localPos + new Vector2(-300, 0);
    }

    public void HideEquipmentInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    #endregion

    #region 🧮 Info Builders

    private string BuildArmorInfo(Armor armor)
    {
        string info = "<b>Type:</b> HullArmor\n";
        if (armor.hpPercent != 0) info += $"+{armor.hpPercent * 100f}% HP\n";
        if (armor.armorPercent != 0) info += $"+{armor.armorPercent * 100f}% Armor\n";
        if (armor.regenFlat != 0) info += $"+{armor.regenFlat} Regen/sec\n";
        return info;
    }

    private string BuildNecklaceInfo(Necklace necklace)
    {
        string info = "<b>Type:</b> Radar\n";
        if (necklace.atkFlat != 0) info += $"+{necklace.atkFlat} ATK\n";
        if (necklace.dodgePercent != 0) info += $"+{necklace.dodgePercent * 100f}% Dodge\n";
        if (necklace.regenFlat != 0) info += $"+{necklace.regenFlat} Regen/sec\n";
        return info;
    }

    private string BuildWeaponInfo(Weapon weapon)
    {
        string info = "<b>Type:</b> Weapon\n";
        if (weapon.AtkFlat != 0) info += $"+{weapon.AtkFlat} ATK\n";
        if (weapon.SpeedPercent != 0) info += $"+{weapon.SpeedPercent * 100f}% Move Speed\n";
        if (weapon.MaxAmmo != 0) info += $"+{weapon.MaxAmmo} Ammo\n";
        if (weapon.BulletSpeed != 0) info += $"+{weapon.BulletSpeed} Bullet Speed\n";
        return info;
    }

    private string BuildShoesInfo(Shoes shoes)
    {
        string info = "<b>Type:</b> Track\n";
        if (shoes.dodgePercent != 0) info += $"+{shoes.dodgePercent * 100f}% Dodge\n";
        if (shoes.speedPercent != 0) info += $"+{shoes.speedPercent * 100f}% Move Speed\n";
        if (shoes.regenFlat != 0) info += $"+{shoes.regenFlat} Regen/sec\n";
        return info;
    }

    private string BuildPantsInfo(Pants pants)
    {
        string info = "<b>Type:</b> Engine\n";
        if (pants.hpPercent != 0) info += $"+{pants.hpPercent * 100f}% HP\n";
        if (pants.armorPercent != 0) info += $"+{pants.armorPercent * 100f}% Armor\n";
        return info;
    }

    private string BuildHatInfo(Hat hat)
    {
        string info = "<b>Type:</b> TurretCover\n";
        if (hat.hpPercent != 0) info += $"+{hat.hpPercent * 100f}% HP\n";
        if (hat.armorPercent != 0) info += $"+{hat.armorPercent * 100f}% Armor\n";
        if (hat.dodgePercent != 0) info += $"+{hat.dodgePercent * 100f}% Dodge\n";
        return info;
    }

    #endregion
}