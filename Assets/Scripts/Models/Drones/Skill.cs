// Tên file: Skill.cs
// Namespace: Assets.Scripts.Models.Skills
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Skills
{
    public enum SkillEffectType
    {
        None, Slow, DamageBuff, HealOverTime, HealFlat, InfiniteAmmo, Invincibility
    }

    public class Skill
    {
        // Thông tin cố định
        public string SkillID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Sprite Icon { get; private set; }
        public SkillEffectType EffectType { get; private set; }

        // Thông tin có thể thay đổi (thời gian, chỉ số)
        public float Cooldown { get; set; } // Cooldown gốc
        public float Duration { get; set; }
        public Dictionary<string, float> Parameters { get; private set; }

        // Trạng thái (để mỗi Drone theo dõi riêng)
        public float CurrentCooldown { get; set; }

        // Constructor (hàm khởi tạo)
        public Skill(string skillID, string name, string desc, Sprite icon,
                     SkillEffectType effectType, float cooldown, float duration,
                     Dictionary<string, float> parameters = null)
        {
            this.SkillID = skillID;
            this.Name = name;
            this.Description = desc;
            this.Icon = icon;
            this.EffectType = effectType;
            this.Cooldown = cooldown;
            this.Duration = duration;
            this.Parameters = parameters ?? new Dictionary<string, float>();
            this.CurrentCooldown = 0f; // Vừa tạo, chưa hồi chiêu
        }

        public float GetParameter(string key, float defaultValue = 0f)
        {
            if (Parameters.TryGetValue(key, out float value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}