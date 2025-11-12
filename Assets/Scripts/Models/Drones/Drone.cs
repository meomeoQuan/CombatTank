// Tên file: Drone.cs
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Models.Equipments;
using Assets.Scripts.Models.Skills;

namespace Assets.Scripts.Models.Drones
{
    public class Drone : EquipmentBase
    {
        // 1. CHỈ SỐ RIÊNG
        public float MaxHP { get; private set; }
        public float CurrentHP { get; set; }
        public float AttackDamage { get; private set; }
        public float DroneArmor { get; private set; }

        // 2. SKILL
        public Dictionary<int, Skill> Skills { get; private set; }

        // 3. THUỘC TÍNH MỚI (DÙNG CHO SHOP)
        public long Cost { get; private set; }

        /// <summary>
        /// Constructor cho Drone
        /// </summary>
        public Drone(string id, string name, Sprite icon,
                     float droneMaxHP, float droneAtk, float droneArmor,
                     long cost, // <<< THÊM MỚI
                     float bonusPlayerHPPercent = 0, float bonusPlayerAtkFlat = 0, float bonusPlayerArmorPercent = 0)
            : base(id, name, icon)
        {
            // Gán chỉ số riêng
            this.MaxHP = droneMaxHP;
            this.CurrentHP = droneMaxHP;
            this.AttackDamage = droneAtk;
            this.DroneArmor = droneArmor;

            this.Cost = cost; // <<< GÁN GIÁ TRỊ MỚI

            // Khởi tạo danh sách skill
            this.Skills = new Dictionary<int, Skill>();

            // 3. CỘNG CHỈ SỐ CHO NHÂN VẬT
            if (bonusPlayerHPPercent != 0) AddPercentBonus(StatType.HP, bonusPlayerHPPercent);
            if (bonusPlayerAtkFlat != 0) AddFlatBonus(StatType.ATK, bonusPlayerAtkFlat);
            if (bonusPlayerArmorPercent != 0) AddPercentBonus(StatType.Armor, bonusPlayerArmorPercent);
        }

        /// <summary>
        /// Gán một kỹ năng vào một "khe cắm" (slot) của Drone
        /// </summary>
        // Dòng này cũng sẽ hoạt động vì đã biết "Skill" là gì
        public void SetSkill(int slot, Skill skill)
        {
            if (skill == null) return;

            if (Skills.ContainsKey(slot))
            {
                Skills[slot] = skill; // Ghi đè nếu đã có skill ở slot này
            }
            else
            {
                Skills.Add(slot, skill); // Thêm mới nếu chưa có
            }
            Debug.Log($"Drone '{Name}' đã gán skill '{skill.Name}' vào slot {slot}");
        }
    }
}