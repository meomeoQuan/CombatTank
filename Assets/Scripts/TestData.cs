using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataController;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Equipments;
using Assets.Scripts.Models; // cho StatType
using System;

public class TestData : MonoBehaviour
{
    void Start()
    {
        // Chỉ Initialize khi chưa có dữ liệu (tránh bị thêm chồng khi Play nhiều lần)
        if (DataController.Characters == null || DataController.Characters.Count == 0)
        {
            DataController.Initialize();
            Debug.Log("[TestData] DataController.Initialize() invoked.");
        }
        else
        {
            Debug.Log("[TestData] DataController already initialized - using existing data.");
        }

        PrintAllData();
    }

    private void PrintAllData()
    {
        Debug.Log("===== TOÀN BỘ DỮ LIỆU TRONG DATA CONTROLLER =====");

        // In master Equipments list (tất cả item đã tạo/đăng ký)
        Debug.Log("---- Master Equipments (DataController.Equipments) ----");
        if (DataController.Equipments == null || DataController.Equipments.Count == 0)
        {
            Debug.Log("No equipments in DataController.Equipments");
        }
        else
        {
            foreach (var eq in DataController.Equipments)
            {
                Debug.Log($"Equip(Id: {GetSafe(() => eq.Id)}, Name: {GetSafe(() => eq.Name)}, Type: {eq.GetType().Name})");
            }
        }

        Debug.Log("---- Characters ----");
        if (DataController.Characters == null || DataController.Characters.Count == 0)
        {
            Debug.Log("No characters in DataController.Characters");
            return;
        }

        foreach (Character character in DataController.Characters)
        {
            PrintCharacterStats(character);
        }

        // ==== TEST: trang bị thêm vũ khí DT10 cho charA ====
        var charA = DataController.Characters[0]; // giả sử A là nhân vật đầu tiên
        var weaponDT10 = DataController.Equipments.Find(e => e.Id == "Gun_DT10");
        if (weaponDT10 != null)
        {
            Debug.Log(">>> Trang bị thêm WeaponDT10 cho Character A để test thay đổi stats <<<");
            charA.Equip(weaponDT10);

            // In lại thông số sau khi trang bị thêm
            PrintCharacterStats(charA);
        }

        Debug.Log("===== END OF DATA DUMP =====");
    }

    private void PrintCharacterStats(Character character)
    {
        Debug.Log($"===== Character: {character.Name} =====");

        // Base stats
        Debug.Log($"  BaseHP: {character.BaseHP}");
        Debug.Log($"  BaseATK: {character.BaseATK}");
        Debug.Log($"  BaseSpeed: {character.BaseSpeed}");
        Debug.Log($"  BaseDodge: {character.BaseDodge}");
        Debug.Log($"  BaseArmor: {character.BaseArmor}");
        Debug.Log($"  BaseRegen: {character.BaseRegen}");

        // Computed stats (tính từ Equipments)
        Debug.Log($"  => HP (computed): {character.HP}");
        Debug.Log($"  => ATK (computed): {character.ATK}");
        Debug.Log($"  => Speed (computed): {character.Speed}");
        Debug.Log($"  => Dodge (computed %): {character.Dodge}%");
        Debug.Log($"  => Armor (computed): {character.Armor}");
        Debug.Log($"  => Regen (computed): {character.Regen}");

        // In toàn bộ trang bị đang đeo
        var equipped = character.GetEquippedItems();
        if (equipped == null)
        {
            Debug.Log("  Trang bị: (không có)");
        }
        else
        {
            var list = new List<EquipmentBase>(equipped);
            if (list.Count == 0)
            {
                Debug.Log("  Trang bị: (không có)");
            }
            else
            {
                Debug.Log("  Trang bị:");
                int idx = 0;
                foreach (var eq in list)
                {
                    idx++;
                    Debug.Log($"   [{idx}] Id: {GetSafe(() => eq.Id)}, Name: {GetSafe(() => eq.Name)}, Type: {eq.GetType().Name}");

                    // In FlatBonuses
                    if (eq.FlatBonuses != null && eq.FlatBonuses.Count > 0)
                    {
                        Debug.Log("      FlatBonuses:");
                        foreach (KeyValuePair<StatType, float> kv in eq.FlatBonuses)
                        {
                            Debug.Log($"        - {kv.Key}: {kv.Value}");
                        }
                    }

                    // In PercentBonuses
                    if (eq.PercentBonuses != null && eq.PercentBonuses.Count > 0)
                    {
                        Debug.Log("      PercentBonuses:");
                        foreach (KeyValuePair<StatType, float> kv in eq.PercentBonuses)
                        {
                            Debug.Log($"        - {kv.Key}: {kv.Value * 100f}%");
                        }
                    }

                    // Nếu là Weapon, in chi tiết trường chuyên biệt (nếu có)
                    var w = eq as Weapon;
                    if (w != null)
                    {
                        Debug.Log("      >> Weapon details:");
                        Debug.Log($"         AtkFlat: {GetSafe(() => w.AtkFlat)}");
                        Debug.Log($"         AtkPercent: {GetSafe(() => w.AtkPercent)}");
                        Debug.Log($"         SpeedPercent: {GetSafe(() => w.SpeedPercent)}");
                        Debug.Log($"         BulletSpeed: {GetSafe(() => w.BulletSpeed)}");
                        Debug.Log($"         ReloadRate: {GetSafe(() => w.ReloadRate)}");
                        Debug.Log($"         MaxAmmo: {GetSafe(() => w.MaxAmmo)}");
                        Debug.Log($"         FireRate: {GetSafe(() => w.FireRate)}");
                    }
                }
            }
        }

        Debug.Log("-----------------------------");
    }


    // Helper: gọi func và bắt exception, trả null string nếu lỗi
    private string GetSafe(System.Func<object> func)
    {
        try
        {
            var val = func();
            return val == null ? "null" : val.ToString();
        }
        catch (System.Exception ex)
        {
            return $"(error: {ex.Message})";
        }
    }
}
