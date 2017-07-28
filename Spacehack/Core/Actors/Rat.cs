using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp.DiceNotation;

namespace Spacehack.Core.Actors
{
    class Rat: Monster
    {
        public static Rat Create(int level)
        {
            int health = Dice.Roll("1D5");
            return new Rat
            {
                Attack = 5 + level,
                AttackChance = Dice.Roll("50D3"),
                Awareness = 10,
                Color = Colors.Gold,
                Defense = (int) (3 + level/1.5),
                DefenseChance = Dice.Roll("4D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Rat",
                Speed = 14,
                Symbol = 'r'
            };
        }
    }
}
