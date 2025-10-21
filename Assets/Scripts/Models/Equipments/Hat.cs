using UnityEngine;

namespace Assets.Scripts.Models.Equipments
{
    // TURNET COVER: THÁP PHÁO
    public class Hat : EquipmentBase
    {
        public Hat(
            string id,
            string name,
            Sprite icon,
            float hpPercent = 0,
            float dodgePercent = 0,
            float armorPercent = 0
        ) : base(id, name, icon)
        {
            if (hpPercent != 0) AddPercentBonus(StatType.HP, hpPercent);
            if (dodgePercent != 0) AddPercentBonus(StatType.Dodge, dodgePercent);
            if (armorPercent != 0) AddPercentBonus(StatType.Armor, armorPercent);
        }
    }
}
