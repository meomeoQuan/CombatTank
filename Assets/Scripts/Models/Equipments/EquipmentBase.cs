using System.Collections.Generic;
using UnityEngine; // cần để dùng Sprite

namespace Assets.Scripts.Models.Equipments
{
    public enum StatType
    {
        HP,
        ATK,
        Speed,
        Dodge,
        Armor,
        Regen
    }

    public abstract class EquipmentBase
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public Sprite Icon { get; protected set; }  // thêm thuộc tính Sprite

        // % bonus (ví dụ +10% HP)
        public Dictionary<StatType, float> PercentBonuses { get; private set; } = new Dictionary<StatType, float>();

        // Cộng trực tiếp (ví dụ +3 ATK)
        public Dictionary<StatType, float> FlatBonuses { get; private set; } = new Dictionary<StatType, float>();

        protected EquipmentBase(string id, string name, Sprite icon)
        {
            Id = id;
            Name = name;
            Icon = icon;
        }

        protected void AddPercentBonus(StatType stat, float value)
        {
            PercentBonuses[stat] = value;
        }

        protected void AddFlatBonus(StatType stat, float value)
        {
            FlatBonuses[stat] = value;
        }
    }
}
