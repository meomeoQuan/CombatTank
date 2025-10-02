using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using Assets.Scripts.Models.Equipments;

namespace Assets.Scripts.Models.Characters
{
    public class Character
    {
        public string Name { get; set; }
        public int BaseHP { get; set; }
        public int BaseATK { get; set; }
        public float BaseSpeed { get; set; }
        public float BaseDodge { get; set; }
        public int BaseArmor { get; set; }
        public int BaseRegen { get; set; }

        // Đảm bảo 1 loại Equipment chỉ có 1 slot
        private Dictionary<Type, EquipmentBase> equippedItems = new();

        // Thuộc tính tính toán chỉ số
        public int HP => (int)(BaseHP * (1 + GetTotalBonusPercent(StatType.HP)) + GetTotalFlat(StatType.HP));
        public int ATK => (int)(BaseATK * (1 + GetTotalBonusPercent(StatType.ATK)) + GetTotalFlat(StatType.ATK));
        public float Speed => BaseSpeed * (1 + GetTotalBonusPercent(StatType.Speed)) + GetTotalFlat(StatType.Speed);
        public float Dodge => BaseDodge * 100f + GetTotalBonusPercent(StatType.Dodge) * 100f;
        public int Armor => (int)(BaseArmor * (1 + GetTotalBonusPercent(StatType.Armor)) + GetTotalFlat(StatType.Armor));
        public int Regen => (int)(BaseRegen * (1 + GetTotalBonusPercent(StatType.Regen)) + GetTotalFlat(StatType.Regen));

        // Equip: tự động thay thế nếu loại đó đã có
        public void Equip(EquipmentBase eq)
        {
            var type = eq.GetType();

            // Nếu đã có cùng loại -> tháo ra trước
            if (equippedItems.ContainsKey(type))
            {
                Unequip(type);
            }

            equippedItems[type] = eq;
        }

        // Unequip theo type
        public void Unequip(Type type)
        {
            if (equippedItems.ContainsKey(type))
            {
                equippedItems.Remove(type);
            }
        }

        // Unequip theo object
        public void Unequip(EquipmentBase eq) => Unequip(eq.GetType());

        // Lấy toàn bộ trang bị hiện tại (nếu cần duyệt)
        public IEnumerable<EquipmentBase> GetEquippedItems() => equippedItems.Values;

        private float GetTotalBonusPercent(StatType stat)
        {
            float total = 0;
            foreach (var eq in equippedItems.Values)
                if (eq.PercentBonuses.ContainsKey(stat))
                    total += eq.PercentBonuses[stat];
            return total;
        }

        private float GetTotalFlat(StatType stat)
        {
            float total = 0;
            foreach (var eq in equippedItems.Values)
                if (eq.FlatBonuses.ContainsKey(stat))
                    total += eq.FlatBonuses[stat];
            return total;
        }
    }
}
