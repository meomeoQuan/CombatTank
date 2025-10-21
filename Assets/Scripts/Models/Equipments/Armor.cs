
using UnityEngine;

namespace Assets.Scripts.Models.Equipments
{
    //  HULL ARMOR: THÂN GIÁP CHÍNH
    public class Armor : EquipmentBase
    {
        public float hpPercent;    // % tăng HP
        public float armorPercent; // % tăng Armor
        public float regenFlat;   // Tăng thẳng máu mỗi giây
        public Armor(string id, string name, Sprite icon, float hpPercent = 0, float armorPercent = 0, float regenFlat = 0)
            : base(id, name, icon)
        {
            this.hpPercent = hpPercent;
            this.armorPercent = armorPercent;
            this.regenFlat = regenFlat;
            if (hpPercent != 0) AddPercentBonus(StatType.HP, hpPercent);
            if (armorPercent != 0) AddPercentBonus(StatType.Armor, armorPercent);
            if (regenFlat != 0) AddFlatBonus(StatType.Regen, regenFlat);
        }
    }
}

