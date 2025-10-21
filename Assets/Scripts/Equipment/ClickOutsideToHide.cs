using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOutsideToHide : MonoBehaviour, IPointerClickHandler
{
    public InventoryUI inventoryUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Nếu click vào khoảng trống, ẩn nút xác nhận
        if (inventoryUI != null)
            inventoryUI.HideConfirmButton();
    }
}
