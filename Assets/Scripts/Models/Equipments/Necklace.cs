using UnityEngine;

namespace Assets.Scripts.Models.Equipments
{
    //RADAR: ĂNG-TEN
    public class Necklace : EquipmentBase
    {
        public Necklace(
            string id,
            string name,
            Sprite icon,
            float atkFlat = 0,
            float dodgePercent = 0,
            float regenFlat = 0
        ) : base(id, name, icon)
        {
            if (atkFlat != 0) AddFlatBonus(StatType.ATK, atkFlat);
            if (regenFlat != 0) AddFlatBonus(StatType.Regen, regenFlat);
            if (dodgePercent != 0) AddPercentBonus(StatType.Dodge, dodgePercent); // sửa Speed → Dodge
        }
    }
}
