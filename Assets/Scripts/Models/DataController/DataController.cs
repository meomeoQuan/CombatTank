using System.Collections.Generic;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Equipments;
using Assets.Scripts.Models.Factory;

namespace Assets.Scripts.DataController
{
    public static class DataController
    {
        public static List<Character> Characters { get; private set; } = new List<Character>();
        public static List<EquipmentBase> Equipments { get; private set; } = new List<EquipmentBase>();

        public static void Initialize()
        {
            // ============================================================
            // 🧩 WEAPONS
            // ============================================================
            var weaponL30A1 = EquipmentFactory.CreateL30A1();
            var weaponDT10 = EquipmentFactory.CreateDT_10();
            var weaponPlasma = EquipmentFactory.CreatePlasma();

            // ============================================================
            // 🛡️ ARMOR
            // ============================================================
            var basicArmor = EquipmentFactory.CreateBasicArmor();
            var reinforcedHull = EquipmentFactory.CreateReinforcedHull();
            var fortifiedHull = EquipmentFactory.CreateFortifiedHull();
            var titaniumHull = EquipmentFactory.CreateTitaniumHull();
            var obsidianHull = EquipmentFactory.CreateObsidianHull();
            var mythicHull = EquipmentFactory.CreateMythicHull();

            // ============================================================
            // ⚙️ ENGINES / PANTS
            // ============================================================
            var basicPants = EquipmentFactory.CreateBasicPants();
            var ironHeart = EquipmentFactory.CreateIronHeart();
            var steelCore = EquipmentFactory.CreateSteelCore();
            var titanDrive = EquipmentFactory.CreateTitanDrive();
            var thunderCore = EquipmentFactory.CreateThunderCore();
            var blazeMotor = EquipmentFactory.CreateBlazeMotor();
            var novaPulse = EquipmentFactory.CreateNovaPulse();
            var phantomCore = EquipmentFactory.CreatePhantomCore();

            // ============================================================
            // 👟 TRACKS / SHOES
            // ============================================================
            var basicShoes = EquipmentFactory.CreateBasicShoes();
            var speedRunner = EquipmentFactory.CreateSpeedRunner();
            var turboDash = EquipmentFactory.CreateTurboDash();
            var phantomStep = EquipmentFactory.CreatePhantomStep();

            // ============================================================
            // 💍 RADAR / NECKLACE
            // ============================================================
            var basicNecklace = EquipmentFactory.CreateBasicNecklace();
            var advancedNecklace = EquipmentFactory.CreateAdvancedNecklace();
            var eliteNecklace = EquipmentFactory.CreateEliteNecklace();

            // ============================================================
            // 🔫 TURRET COVERS
            // ============================================================
            var basicHat = EquipmentFactory.CreateBasicHat();
            var opticsTurret = EquipmentFactory.CreateOpticsTurret();
            var gunneryTurret = EquipmentFactory.CreateGunneryTurret();
            var heavyTurret = EquipmentFactory.CreateHeavyTurret();
            var ballTurret = EquipmentFactory.CreateBallTurret();
            var sentryTurret = EquipmentFactory.CreateSentryTurret();
            var gatlingTurret = EquipmentFactory.CreateGatlingTurret();
            var targetTurret = EquipmentFactory.CreateTargetAcquisitionTurret();

            // ============================================================
            // 📦 ADD ALL EQUIPMENTS
            // ============================================================
            Equipments.AddRange(new List<EquipmentBase>
            {
                // Weapons
                weaponL30A1,
                weaponDT10,
                weaponPlasma,

                // Armor
                basicArmor,
                reinforcedHull,
                fortifiedHull,
                titaniumHull,
                obsidianHull,
                mythicHull,

                // Engine
                basicPants,
                ironHeart,
                steelCore,
                titanDrive,
                thunderCore,
                blazeMotor,
                novaPulse,
                phantomCore,

                // Shoes
                basicShoes,
                speedRunner,
                turboDash,
                phantomStep,

                // Necklace
                basicNecklace,
                advancedNecklace,
                eliteNecklace,



                // Turrets
                basicHat,
                opticsTurret,
                gunneryTurret,
                heavyTurret,
                ballTurret,
                sentryTurret,
                gatlingTurret,
                targetTurret
            });

            // ============================================================
            // 👤 CHARACTERS
            // ============================================================
            var charA = CharacterFactory.CreateCharacterA();
            var charB = CharacterFactory.CreateCharacterB();

            // Character A default equips
            charA.Equip(weaponPlasma);
            charA.Equip(basicHat);
            charA.Equip(basicShoes);
            charA.Equip(basicPants);
            charA.Equip(basicArmor);

            // Character B default equips
            charB.Equip(weaponPlasma);
            charB.Equip(opticsTurret);
            charB.Equip(basicShoes);
            charB.Equip(ironHeart);
            charB.Equip(basicArmor);
            charB.Equip(basicNecklace);

            // ============================================================
            // 📋 ADD CHARACTERS
            // ============================================================
            Characters.Add(charA);
            Characters.Add(charB);
        }
    }
}
