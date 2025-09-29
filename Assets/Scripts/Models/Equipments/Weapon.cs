using UnityEngine;

namespace Assets.Scripts.Models.Equipments
{
    // CANNON: PHÁO CHÍNH
    public class Weapon : EquipmentBase
    {
        public float AtkFlat { get; private set; }
        public float AtkPercent { get; private set; }
        public float SpeedPercent { get; private set; }

        public float BulletSpeed { get; private set; } // tốc độ bay của viên đạn
        public float ReloadRate { get; private set; }  // mỗi giây hồi được bao nhiêu viên đạn
        public int Ammo { get; private set; }          // số lượng đạn hiện tại
        public int MaxAmmo { get; private set; }       // băng đạn tối đa

        public float FireRate { get; private set; }    // số phát bắn mỗi giây

        public Weapon(
            string id,
            string name,
            Sprite icon,
            float atkFlat = 0,
            float atkPercent = 0,
            float speedPercent = 0,
            float bulletSpeed = 10f,
            float reloadRate = 1f,   // mỗi giây hồi 1 viên
            int maxAmmo = 30,
            float fireRate = 2f      // bắn được 2 phát/giây
        ) : base(id, name, icon)
        {
            // Thêm chỉ số tấn công và tốc độ di chuyển vào hệ thống bonus
            if (atkFlat != 0) AddFlatBonus(StatType.ATK, atkFlat);
            if (atkPercent != 0) AddPercentBonus(StatType.ATK, atkPercent);
            if (speedPercent != 0) AddFlatBonus(StatType.Speed, speedPercent);
            // Lưu thuộc tính vũ khí vào property
            AtkFlat = atkFlat;
            AtkPercent = atkPercent;
            SpeedPercent = speedPercent;


            // Thuộc tính riêng của vũ khí
            BulletSpeed = bulletSpeed;
            ReloadRate = reloadRate;
            MaxAmmo = maxAmmo;
            Ammo = maxAmmo; // khi tạo vũ khí thì đầy đạn
            FireRate = fireRate;
        }

        // Hồi đạn theo thời gian
        public void RegenerateAmmo(float deltaTime)
        {
            Ammo += (int)(ReloadRate * deltaTime);
            if (Ammo > MaxAmmo) Ammo = MaxAmmo;
        }

        // Bắn
        public bool Shoot()
        {
            if (Ammo > 0)
            {
                Ammo--;
                return true; // bắn thành công
            }
            return false; // hết đạn
        }
    }
}
