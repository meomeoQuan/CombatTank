using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Equipments;

namespace Assets.Scripts.Models.Factory
{
    public static class CharacterFactory
    {
        public static Character CreateCharacterA()
        {
            var character = new Character
            {
                Name = "A",
                BaseHP = 50,
                BaseATK = 15,
                BaseSpeed = 2,
                BaseDodge = 0,
                BaseArmor = 10,
                BaseRegen = 0
            };


            return character;
        }

        public static Character CreateCharacterB()
        {
            var character = new Character
            {
                Name = "B",
                BaseHP = 60,
                BaseATK = 14,
                BaseSpeed = 2,
                BaseDodge = 0,
                BaseArmor = 10,
                BaseRegen = 0
            };



            return character;
        }
    }
}
