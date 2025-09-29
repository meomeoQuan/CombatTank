using Assets.Scripts.Models;
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

        public List<EquipmentBase> Equipments { get; private set; } = new List<EquipmentBase>();

        public int HP => (int)(BaseHP * (1 + GetTotalBonusPercent(StatType.HP)) + GetTotalFlat(StatType.HP));
        public int ATK => (int)(BaseATK * (1 + GetTotalBonusPercent(StatType.ATK)) + GetTotalFlat(StatType.ATK));
        public float Speed => BaseSpeed * (1 + GetTotalBonusPercent(StatType.Speed)) + GetTotalFlat(StatType.Speed);
        public float Dodge => BaseDodge * 100f + GetTotalBonusPercent(StatType.Dodge) * 100f;
        public int Armor => (int)(BaseArmor * (1 + GetTotalBonusPercent(StatType.Armor)) + GetTotalFlat(StatType.Armor));
        public int Regen => (int)(BaseRegen * (1 + GetTotalBonusPercent(StatType.Regen)) + GetTotalFlat(StatType.Regen));

        public void Equip(EquipmentBase eq) => Equipments.Add(eq);
        public void Unequip(EquipmentBase eq) => Equipments.Remove(eq);

        private float GetTotalBonusPercent(StatType stat)
        {
            float total = 0;
            foreach (var eq in Equipments)
                if (eq.PercentBonuses.ContainsKey(stat))
                    total += eq.PercentBonuses[stat];
            return total;
        }

        private float GetTotalFlat(StatType stat)
        {
            float total = 0;
            foreach (var eq in Equipments)
                if (eq.FlatBonuses.ContainsKey(stat))
                    total += eq.FlatBonuses[stat];
            return total;
        }
    }
}
