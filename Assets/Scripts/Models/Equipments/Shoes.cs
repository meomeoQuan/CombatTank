using UnityEngine;

namespace Assets.Scripts.Models.Equipments
{
    public class Shoes : EquipmentBase
    {
        // TRACKS: BÁNH XÍCH
        public Shoes(
            string id,
            string name,
            Sprite icon,
            float speedPercent = 0,
            float dodgePercent = 0,
            float regenFlat = 0
        ) : base(id, name, icon)
        {
            if (speedPercent != 0) AddPercentBonus(StatType.Speed, speedPercent);
            if (dodgePercent != 0) AddPercentBonus(StatType.Dodge, dodgePercent);
            if (regenFlat != 0) AddFlatBonus(StatType.Regen, regenFlat);
        }
    }
}
