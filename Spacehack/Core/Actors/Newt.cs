using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp.DiceNotation;
using RLNET;

namespace Spacehack.Core
{
    public class Newt: Monster
    {
        public static Newt Create(int level)
        {
            int health = Dice.Roll("1D5");
            return new Newt
            {
                Attack = 1 + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.Gold,
                Defense = 1 + level / 3,
                DefenseChance = Dice.Roll("1D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Newt",
                Speed = 14,
                Symbol = ':'
            };
        }
    }
}
