using System.Collections.Generic;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Equipments;
using Assets.Scripts.Models.Factory;

namespace Assets.Scripts.DataController
{
    public static class DataController
    {
        // Danh sách nhân vật
        public static List<Character> Characters { get; private set; } = new List<Character>();

        // Danh sách trang bị
        public static List<EquipmentBase> Equipments { get; private set; } = new List<EquipmentBase>();

        public static void Initialize()
        {
            // ====== Tạo trang bị mặc định ======
            var WeaponL30A1 = EquipmentFactory.CreateL30A1();
            var WeaponDT10 = EquipmentFactory.CreateDT_10();
            var WeaponPlasmaRifle = EquipmentFactory.CreatePlasma();
            var basicArmor = EquipmentFactory.CreateBasicArmor();
            var basicPants = EquipmentFactory.CreateBasicPants();
            var basicShoes = EquipmentFactory.CreateBasicShoes();
            var basicNecklace = EquipmentFactory.CreateBasicNecklace();
            var basicHat = EquipmentFactory.CreateBasicHat();


            // Lưu tất cả trang bị vào list
            Equipments.Add(WeaponL30A1);
            Equipments.Add(WeaponDT10);
            Equipments.Add(WeaponPlasmaRifle);
            Equipments.Add(basicArmor);
            Equipments.Add(basicPants);
            Equipments.Add(basicShoes);
            Equipments.Add(basicNecklace);
            Equipments.Add(basicHat);

            // ====== Tạo nhân vật ======
            var charA = CharacterFactory.CreateCharacterA();
            var charB = CharacterFactory.CreateCharacterB();

            // Trang bị mặc định cho nhân vật A
            charA.Equip(WeaponL30A1);


            // Trang bị mặc định cho nhân vật B 
            charB.Equip(WeaponDT10);


            // Thêm nhân vật vào list
            Characters.Add(charA);
            Characters.Add(charB);
        }
    }
}
