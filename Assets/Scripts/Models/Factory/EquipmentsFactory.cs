using Assets.Scripts.Models.Equipments;
using UnityEngine;

namespace Assets.Scripts.Models.Factory
{
    public static class EquipmentFactory
    {
        private static Sprite LoadIcon(string fileName)
        {
            return Resources.Load<Sprite>($"Equipments/{fileName}");
        }

        // Armor (Áo)
        public static Armor CreateBasicArmor()
        {
            return new Armor(
                id: "armor_basic",
                name: "Basic Armor",
                icon: LoadIcon("armor_icon"),
                hpPercent: 0.1f,
                armorPercent: 0.3f
            );
        }

        // Pants (Quần)
        public static Pants CreateBasicPants()
        {
            return new Pants(
                id: "pants_basic",
                name: "Basic Pants",
                icon: LoadIcon("pants_icon"),
                hpPercent: 0.2f,
                armorPercent: 0.2f
            );
        }

        // Shoes (Giày)
        public static Shoes CreateBasicShoes()
        {
            return new Shoes(
                id: "shoes_basic",
                name: "Basic Shoes",
                icon: LoadIcon("shoes_icon"),
                speedPercent: 0.3f,
                dodgePercent: 0.03f,
                regenFlat: 1
            );
        }

        // Necklace (Dây chuyền)
        public static Necklace CreateBasicNecklace()
        {
            return new Necklace(
                id: "necklace_basic",
                name: "Basic Necklace",
                icon: LoadIcon("necklace_icon"),
                atkFlat: 1,
                dodgePercent: 0.01f,
                regenFlat: 2
            );
        }

        // Hat (Mũ)
        public static Hat CreateBasicHat()
        {
            return new Hat(
                id: "hat_basic",
                name: "Basic Hat",
                icon: LoadIcon("hat_icon"),
                hpPercent: 0.3f,
                armorPercent: 0.1f
            );
        }

        // Weapon 
        public static Weapon CreateL30A1()
        {
            return new Weapon(
                id: "Cannon_L30A1",
                name: "Cannon",
                icon: LoadIcon("Cannon_L30A1"),
                atkFlat: 2,
                atkPercent: 0,
                speedPercent: 0.1f,
                bulletSpeed: 10f,
                reloadRate: 3f,
                maxAmmo: 15,
                fireRate: 2f
            );
        }

        public static Weapon CreateDT_10()
        {
            return new Weapon(
                id: "Gun_DT10",
                name: "Gun",
                icon: LoadIcon("Gun_DT10"),
                atkFlat: 4,
                atkPercent: 0,
                speedPercent: 0.15f,
                bulletSpeed: 12f,
                reloadRate: 4f,
                maxAmmo: 20,
                fireRate: 5f
            );
        }
        public static Weapon CreatePlasma()
        {
            return new Weapon(
                id: "Gun_Plasma",
                name: "Plasma",
                icon: LoadIcon("Gun_Plasma"),
                atkFlat: 2,
                atkPercent: 0,
                speedPercent: 0.15f,
                bulletSpeed: 15f,
                reloadRate: 3f,
                maxAmmo: 30,
                fireRate: 5f
            );
        }
    }
}
