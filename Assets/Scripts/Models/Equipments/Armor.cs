using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.AppUI.UI;
using UnityEngine;

namespace Assets.Scripts.Models.Equipments
{
    //  HULL ARMOR: THÂN GIÁP CHÍNH
    public class Armor : EquipmentBase
    {
        public Armor(string id, string name, Sprite icon, float hpPercent = 0, float armorPercent = 0, float regenFlat = 0)
            : base(id, name, icon)
        {
            if (hpPercent != 0) AddPercentBonus(StatType.HP, hpPercent);
            if (armorPercent != 0) AddPercentBonus(StatType.Armor, armorPercent);
            if (regenFlat != 0) AddFlatBonus(StatType.Regen, regenFlat);
        }
    }
}

