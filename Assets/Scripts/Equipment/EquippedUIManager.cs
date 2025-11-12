using Assets.Scripts.DataController;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Equipments;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedUIManager : MonoBehaviour
{
    [Header("Player Switch UI")]
    [SerializeField] private TMP_Dropdown playerDropdown;
    public CharacterInfoUI characterInfoUI;

    [Header("Equipment Slots")]
    [SerializeField] private EquipmentSlotUI weaponSlot;
    [SerializeField] private EquipmentSlotUI hullSlot;
    [SerializeField] private EquipmentSlotUI trackSlot;
    [SerializeField] private EquipmentSlotUI engineSlot;
    [SerializeField] private EquipmentSlotUI radarSlot;
    [SerializeField] private EquipmentSlotUI turretSlot;

    [Header("References")]
    [SerializeField] private InventoryUI inventoryUI; 

    private List<Character> characters;
    private Character currentCharacter;

    private void Awake()
    {
        //==============================NHỚ XÓA========================//
        if (DataController.Equipments == null || DataController.Equipments.Count == 0)
            DataController.Initialize();
        //==============================================================//
    }
    private void Start()
    {
        

        characters = DataController.Characters;

        playerDropdown.ClearOptions();
        var options = new List<string>();
        for (int i = 0; i < characters.Count; i++)
            options.Add($"Player {i + 1}");
        playerDropdown.AddOptions(options);
        playerDropdown.onValueChanged.AddListener(OnPlayerChanged);
        if (inventoryUI == null)
            Debug.LogWarning("[EquippedUIManager] inventoryUI chưa gán trong Inspector.");
        OnPlayerChanged(0);
    }

    private void OnDestroy()
    {
        playerDropdown.onValueChanged.RemoveListener(OnPlayerChanged);
    }

    private void OnPlayerChanged(int index)
    {
        if (index < 0 || index >= characters.Count)
            return;

        currentCharacter = characters[index];
        UpdateEquippedSlots();
        if (inventoryUI != null)
            inventoryUI.RefreshUI(currentCharacter);
        else
            Debug.LogWarning("[EquippedUIManager] inventoryUI null khi cố gắng refresh.");
    }

    public Character GetCurrentCharacter() => currentCharacter;

    public void RefreshCurrentCharacter()
    {
        UpdateEquippedSlots();
        inventoryUI.RefreshUI(currentCharacter);
    }

    private void UpdateEquippedSlots()
    {
        // Clear và setup callback
        weaponSlot.Setup(null, (e) => OnSlotClicked(typeof(Weapon), null));
        hullSlot.Setup(null, (e) => OnSlotClicked(typeof(Armor), null));
        trackSlot.Setup(null, (e) => OnSlotClicked(typeof(Shoes), null));
        engineSlot.Setup(null, (e) => OnSlotClicked(typeof(Pants), null));
        radarSlot.Setup(null, (e) => OnSlotClicked(typeof(Necklace), null));
        turretSlot.Setup(null, (e) => OnSlotClicked(typeof(Hat), null));

        foreach (var kvp in currentCharacter.EquippedItems)
        {
            var eqType = kvp.Key;
            var eq = kvp.Value;

            if (eq == null) continue;

            if (eqType == typeof(Weapon))
                weaponSlot.Setup(eq, (e) => OnSlotClicked(eqType, e), inventoryUI);
            else if (eqType == typeof(Armor))
                hullSlot.Setup(eq, (e) => OnSlotClicked(eqType, e), inventoryUI);
            else if (eqType == typeof(Shoes))
                trackSlot.Setup(eq, (e) => OnSlotClicked(eqType, e), inventoryUI);
            else if (eqType == typeof(Pants))
                engineSlot.Setup(eq, (e) => OnSlotClicked(eqType, e), inventoryUI);
            else if (eqType == typeof(Necklace))
                radarSlot.Setup(eq, (e) => OnSlotClicked(eqType, e), inventoryUI);
            else if (eqType == typeof(Hat))
                turretSlot.Setup(eq, (e) => OnSlotClicked(eqType, e), inventoryUI);
        }
    }

    private void OnSlotClicked(System.Type type, EquipmentBase equipment = null)
    {
        if (equipment != null)
        {
            // có trang bị -> Unequip luôn
            Debug.Log($"Unequipped {equipment.Name}");
            currentCharacter.Unequip(equipment);
            RefreshCurrentCharacter();
        }
        else
        {
            // slot trống -> lọc inventory theo loại tương ứng
            Debug.Log($"Slot rỗng ({type.Name}) → lọc danh sách");
            inventoryUI.FilterByType(type, currentCharacter);
        }
    }
    public void OnShowCharacterInfo()
    {
        characterInfoUI.Show(currentCharacter);
    }
}