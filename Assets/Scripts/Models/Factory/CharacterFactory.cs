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
                BaseHP = 100,
                BaseATK = 5,
                BaseSpeed = 2,
                BaseDodge = 0,
                BaseArmor = 1,
                BaseRegen = 0
            };


            return character;
        }

        public static Character CreateCharacterB()
        {
            var character = new Character
            {
                Name = "B",
                BaseHP = 120,
                BaseATK = 4,
                BaseSpeed = 2,
                BaseDodge = 0,
                BaseArmor = 1,
                BaseRegen = 0
            };



            return character;
        }
    }
}
