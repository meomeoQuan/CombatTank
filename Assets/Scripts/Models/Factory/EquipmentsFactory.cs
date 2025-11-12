using Assets.Scripts.Models.Equipments;
using Assets.Scripts.Models.Skills;
using Assets.Scripts.Models.Drones;
using UnityEngine;

namespace Assets.Scripts.Models.Factory
{
    public static class EquipmentFactory
    {
        private static Sprite LoadIcon(string fileName)
        {
            return Resources.Load<Sprite>($"Equipments/{fileName}");
        }
        private static Sprite LoadDrone(string fileName)
        {
            return Resources.Load<Sprite>($"Equipments/Drones/{fileName}");
        }

        // Armor (Áo)
        #region Create Armor
        public static Armor CreateBasicArmor()
        {
            return new Armor(
                id: "armor_basic",
                name: "Basic Armor",
                icon: LoadIcon("Basic_HullArmor"),
                hpPercent: 0.10f,
                armorPercent: 0.3f
            );
        }

        public static Armor CreateReinforcedHull()
        {
            return new Armor(
                id: "armor_hull_2",
                name: "Reinforced Hull",
                icon: LoadIcon("Hull_02"),
                hpPercent: 0.15f,
                armorPercent: 0.40f
            );
        }

        public static Armor CreateFortifiedHull()
        {
            return new Armor(
                id: "armor_hull_3",
                name: "Fortified Hull",
                icon: LoadIcon("Hull_03"),
                hpPercent: 0.20f,
                armorPercent: 0.50f
            );
        }

        public static Armor CreateTitaniumHull()
        {
            return new Armor(
                id: "armor_hull_4",
                name: "Titanium Hull",
                icon: LoadIcon("Hull_04"),
                hpPercent: 0.25f,
                armorPercent: 0.60f,
                regenFlat: 1
            );
        }

        public static Armor CreateObsidianHull()
        {
            return new Armor(
                id: "armor_hull_5",
                name: "Obsidian Hull",
                icon: LoadIcon("Hull_05"),
                hpPercent: 0.30f,
                armorPercent: 0.75f,
                regenFlat: 2
            );
        }

        public static Armor CreateMythicHull()
        {
            return new Armor(
                id: "armor_hull_6",
                name: "Mythic Hull",
                icon: LoadIcon("Hull_06"),
                hpPercent: 0.40f,
                armorPercent: 0.90f,
                regenFlat: 3
            );
        }
        #endregion
        // Pants (Quần)
        #region Create Engine

        public static Pants CreateBasicPants()
        {
            return new Pants(
                id: "engine_basic",
                name: "Basic Engine",
                icon: LoadIcon("Engine_1"),
                hpPercent: 0.2f,
                armorPercent: 0.2f
            );
        }
        public static Pants CreateIronHeart()
        {
            return new Pants(
                id: "engine_ironheart",
                name: "Iron Heart",
                icon: LoadIcon("Engine_2"),
                hpPercent: 0.3f,
                armorPercent: 0.3f
            );
        }

        public static Pants CreateSteelCore()
        {
            return new Pants(
                id: "engine_steelcore",
                name: "Steel Core",
                icon: LoadIcon("Engine_3"),
                hpPercent: 0.4f,
                armorPercent: 0.4f
            );
        }

        public static Pants CreateTitanDrive()
        {
            return new Pants(
                id: "engine_titandrive",
                name: "Titan Drive",
                icon: LoadIcon("Engine_4"),
                hpPercent: 0.5f,
                armorPercent: 0.5f
            );
        }

        public static Pants CreateThunderCore()
        {
            return new Pants(
                id: "engine_thundercore",
                name: "Thunder Core",
                icon: LoadIcon("Engine_5"),
                hpPercent: 0.6f,
                armorPercent: 0.6f
            );
        }

        public static Pants CreateBlazeMotor()
        {
            return new Pants(
                id: "engine_blazemotor",
                name: "Blaze Motor",
                icon: LoadIcon("Engine_6"),
                hpPercent: 0.7f,
                armorPercent: 0.7f
            );
        }

        public static Pants CreateNovaPulse()
        {
            return new Pants(
                id: "engine_novapulse",
                name: "Nova Pulse",
                icon: LoadIcon("Engine_7"),
                hpPercent: 0.8f,
                armorPercent: 0.8f
            );
        }

        public static Pants CreatePhantomCore()
        {
            return new Pants(
                id: "engine_phantomcore",
                name: "Phantom Core",
                icon: LoadIcon("Engine_8"),
                hpPercent: 0.9f,
                armorPercent: 0.9f
            );
        }
        #endregion
        // Shoes (Giày)
        #region Create Track

        public static Shoes CreateBasicShoes()
        {
            return new Shoes(
                id: "shoes_basic",
                name: "Basic Track",
                icon: LoadIcon("Basic_Track"),
                speedPercent: 0.3f,
                dodgePercent: 0.03f,
                regenFlat: 1
            );
        }
        public static Shoes CreateSpeedRunner()
        {
            return new Shoes(
                id: "shoes_speedrunner",
                name: "Speed Runner",
                icon: LoadIcon("track_2"),
                speedPercent: 0.4f,    // tăng nhẹ tốc độ
                dodgePercent: 0.05f,   // tăng né tránh
                regenFlat: 2           // hồi máu cao hơn
            );
        }

        public static Shoes CreateTurboDash()
        {
            return new Shoes(
                id: "shoes_turbodash",
                name: "Turbo Dash",
                icon: LoadIcon("track_3"),
                speedPercent: 0.55f,
                dodgePercent: 0.07f,
                regenFlat: 3
            );
        }

        public static Shoes CreatePhantomStep()
        {
            return new Shoes(
                id: "shoes_phantomstep",
                name: "Phantom Step",
                icon: LoadIcon("track_4"),
                speedPercent: 0.7f,
                dodgePercent: 0.1f,
                regenFlat: 4
            );
        }

        #endregion
        // Necklace (Dây chuyền)
        #region Create Necklace
        public static Necklace CreateBasicNecklace()
        {
            return new Necklace(
                id: "necklace_basic",
                name: "Basic Necklace",
                icon: LoadIcon("Radar_1"),
                atkFlat: 1,
                dodgePercent: 0.01f,
                regenFlat: 2
            );
        }

        public static Necklace CreateAdvancedNecklace()
        {
            return new Necklace(
                id: "necklace_advanced",
                name: "Advanced Necklace",
                icon: LoadIcon("Radar_2"),
                atkFlat: 2,
                dodgePercent: 0.02f,
                regenFlat: 3
            );
        }

        public static Necklace CreateEliteNecklace()
        {
            return new Necklace(
                id: "necklace_elite",
                name: "Elite Necklace",
                icon: LoadIcon("Radar_3"),
                atkFlat: 4,
                dodgePercent: 0.04f,
                regenFlat: 5
            );
        }

        #endregion
        // Hat (Mũ)
        #region Turret Cover
        public static Hat CreateBasicHat()
        {
            return new Hat(
                id: "hat_basic",
                name: "Basic Turret",
                icon: LoadIcon("Basic_TurretCover"),
                hpPercent: 0.3f,
                armorPercent: 0.1f
            );
        }
        public static Hat CreateOpticsTurret()
        {
            return new Hat(
                id: "hat_optics",
                name: "Optics Turret",
                icon: LoadIcon("Optics_TurretCover"),
                hpPercent: 0.4f,
                armorPercent: 0.2f
            );
        }

        public static Hat CreateGunneryTurret()
        {
            return new Hat(
                id: "hat_gunnery",
                name: "Gunnery Turret",
                icon: LoadIcon("Gunnery_TurretCover"),
                hpPercent: 0.5f,
                armorPercent: 0.3f
            );
        }

        public static Hat CreateHeavyTurret()
        {
            return new Hat(
                id: "hat_heavy",
                name: "Heavy Turret",
                icon: LoadIcon("Heavy_TurretCover"),
                hpPercent: 0.6f,
                armorPercent: 0.4f
            );
        }

        public static Hat CreateBallTurret()
        {
            return new Hat(
                id: "hat_ball",
                name: "Ball Turret",
                icon: LoadIcon("Ball_TurretCover"),
                hpPercent: 0.7f,
                armorPercent: 0.5f
            );
        }

        public static Hat CreateSentryTurret()
        {
            return new Hat(
                id: "hat_sentry",
                name: "Sentry Turret",
                icon: LoadIcon("Sentry_TurretCover"),
                hpPercent: 0.8f,
                armorPercent: 0.6f
            );
        }

        public static Hat CreateGatlingTurret()
        {
            return new Hat(
                id: "hat_gatling",
                name: "Gatling Turret",
                icon: LoadIcon("Gatling_TurretCover"),
                hpPercent: 0.9f,
                armorPercent: 0.7f
            );
        }

        public static Hat CreateTargetAcquisitionTurret()
        {
            return new Hat(
                id: "hat_target",
                name: "Acquisition Turret",
                icon: LoadIcon("TargetAcquisition_TurretCover"),
                hpPercent: 1.0f,
                armorPercent: 0.8f
            );
        }
        #endregion
        // Weapon 
        #region Weapon
        public static Weapon CreateL30A1()
        {
            return new Weapon(
                id: "Cannon",
                name: "Cannon L30A1",
                icon: LoadIcon("Cannon_L30A1"),
                atkFlat: 8,
                atkPercent: 0.1f,
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
                id: "Gun",
                name: "Gun DT10",
                icon: LoadIcon("Gun_DT10"),
                atkFlat: 15,
                atkPercent: 0.15f,
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
                id: "Plasma",
                name: "Plasma",
                icon: LoadIcon("Gun_Plasma"),
                atkFlat: 12,
                atkPercent: 0.15f,
                speedPercent: 0.15f,
                bulletSpeed: 15f,
                reloadRate: 3f,
                maxAmmo: 30,
                fireRate: 5f
            );
        }
        #endregion
        // Drone
        #region Drone

        public static Drone CreateSupportDrone()
        {
            Drone drone = new Drone(
                id: "drone_support_01",
                name: "Support Drone MK1",
                icon: LoadDrone("DroneSupport"),
                droneMaxHP: 100f,
                droneAtk: 5f,
                droneArmor: 10f,
                cost: 10, 
                unlockCost: 0,
                bonusPlayerHPPercent: 0.10f,
                bonusPlayerArmorPercent: 0.05f
            );

            // GỌI SKILL FACTORY
            //Skill slow = SkillFactory.CreateSlowSkill(slowPercent: 0.3f, duration: 3f, cooldown: 12f);
            Skill heal = SkillFactory.CreateHealSkill(healAmount: 25f, cooldown: 20f);

            //drone.SetSkill(1, slow);
            drone.SetSkill(2, heal);

            return drone;
        }

        // Ví dụ Drone A của bạn
        public static Drone CreateCustomDroneA()
        {
            Drone droneA = new Drone(
                id: "drone_A_custom",
                name: "Drone A",
                icon: LoadDrone("DroneAttack"),
                droneMaxHP: 100f,
                droneAtk: 10f,
                droneArmor: 10f,
                cost: 50, 
                unlockCost: 500,
                bonusPlayerHPPercent: 0.1f,
                bonusPlayerAtkFlat: 5
            );

            // TẠO SKILL TÙY BIẾN
            Skill customSkill = SkillFactory.CreateSlowSkill(
                slowPercent: 0.50f,
                duration: 3f,
                cooldown: 5f
            );

            droneA.SetSkill(1, customSkill);

            return droneA;
        }

        #endregion
    }
}
