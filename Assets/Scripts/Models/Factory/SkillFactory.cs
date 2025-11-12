// Tên file: SkillFactory.cs
// Namespace: Assets.Scripts.Models.Factory
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Models.Skills; // Cần using để biết class Skill

namespace Assets.Scripts.Models.Factory
{
    /// <summary>
    /// Lớp static chuyên để tạo ra các "instance" Skill
    /// với các chỉ số tùy biến được truyền vào.
    /// </summary>
    public static class SkillFactory
    {
        // Hàm helper để tải icon
        private static Sprite LoadIcon(string fileName)
        {
            // Giả sử icon skill nằm trong thư mục con "Skills/"
            return Resources.Load<Sprite>($"Equipments/Skills/{fileName}");
        }

        /// <summary>
        /// 1. TẠO SKILL LÀM CHẬM
        /// </summary>
        public static Skill CreateSlowSkill(float slowPercent, float duration, float cooldown)
        {
            var parameters = new Dictionary<string, float>
            {
                { "SlowPercent", slowPercent } // Ví dụ: 0.5f cho 50%
            };

            return new Skill(
                skillID: "skill_slow",
                name: "Cryo Shot",
                desc: $"Làm chậm kẻ thù {slowPercent * 100}% trong {duration} giây.",
                icon: LoadIcon("Skill_Cryo"),
                effectType: SkillEffectType.Slow,
                cooldown: cooldown,
                duration: duration,
                parameters: parameters
            );
        }

        /// <summary>
        /// 2. TẠO SKILL TĂNG SÁT THƯƠNG
        /// </summary>
        public static Skill CreateDamageBuffSkill(float damageFlat, float duration, float cooldown)
        {
            var parameters = new Dictionary<string, float>
            {
                { "DamageFlat", damageFlat }
            };

            return new Skill(
                skillID: "skill_dmg_buff",
                name: "Overcharge",
                desc: $"Tăng +{damageFlat} sát thương trong {duration} giây.",
                icon: LoadIcon("Skill_DmgBuff"),
                effectType: SkillEffectType.DamageBuff,
                cooldown: cooldown,
                duration: duration,
                parameters: parameters
            );
        }

        /// <summary>
        /// 3. TẠO SKILL HỒI MÁU (TỨC THỜI)
        /// </summary>
        public static Skill CreateHealSkill(float healAmount, float cooldown)
        {
            var parameters = new Dictionary<string, float>
            {
                { "HealAmount", healAmount }
            };

            return new Skill(
                skillID: "skill_heal_flat",
                name: "Repair Nanites",
                desc: $"Hồi {healAmount} HP ngay lập tức.",
                icon: LoadIcon("Skill_Heal"),
                effectType: SkillEffectType.HealFlat,
                cooldown: cooldown,
                duration: 0f, // Tức thời, không có duration
                parameters: parameters
            );
        }

        /// <summary>
        /// 4. TẠO SKILL ĐẠN VÔ HẠN
        /// </summary>
        public static Skill CreateInfiniteAmmoSkill(float duration, float cooldown)
        {
            return new Skill(
                skillID: "skill_inf_ammo",
                name: "Unlimited Ammo",
                desc: $"Không tốn đạn trong {duration} giây.",
                icon: LoadIcon("Skill_InfAmmo"),
                effectType: SkillEffectType.InfiniteAmmo,
                cooldown: cooldown,
                duration: duration
            // Không cần 'parameters'
            );
        }

        /// <summary>
        /// 5. TẠO SKILL BẤT TỬ
        /// </summary>
        public static Skill CreateInvincibilitySkill(float duration, float cooldown)
        {
            return new Skill(
                skillID: "skill_invincible",
                name: "Aegis Shield",
                desc: $"Trở nên bất tử trong {duration} giây.",
                icon: LoadIcon("Skill_Invincible"),
                effectType: SkillEffectType.Invincibility,
                cooldown: cooldown,
                duration: duration
            );
        }
    }
}