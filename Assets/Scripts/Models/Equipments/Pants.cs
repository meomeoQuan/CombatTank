using UnityEngine;

namespace Assets.Scripts.Models.Equipments
{
    //ENGINE: HỆ THỐNG ĐỘNG CƠ 
    public class Pants : EquipmentBase
    {
        public float  hpPercent;    // % tăng HP
        public float armorPercent; // % tăng Armor
        public Pants(
            string id,
            string name,
            Sprite icon,
            float hpPercent = 0,
            float armorPercent = 0
        ) : base(id, name, icon)
        {
            this.hpPercent = hpPercent;
            this.armorPercent = armorPercent;
            if (hpPercent != 0) AddPercentBonus(StatType.HP, hpPercent);
            if (armorPercent != 0) AddPercentBonus(StatType.Armor, armorPercent);
        }
    }
}
