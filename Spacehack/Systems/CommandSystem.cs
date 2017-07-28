using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp.DiceNotation;
using Spacehack.Interfaces;
using RogueSharp;
using System.Threading.Tasks;

namespace Spacehack.Core
{
    public class CommandSystem
    {
        // Return value is true if the player was able to move
        // false when the player couldn't move, such as trying to move into a wall
        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            switch (direction)
            {
                case Direction.Up:
                    {
                        y = Game.Player.Y - 1;
                        break;
                    }
                case Direction.Down:
                    {
                        y = Game.Player.Y + 1;
                        break;
                    }
                case Direction.Left:
                    {
                        x = Game.Player.X - 1;
                        break;
                    }
                case Direction.Right:
                    {
                        x = Game.Player.X + 1;
                        break;
                    }
                case Direction.UpRight:
                    {
                        y = Game.Player.Y - 1;
                        x = Game.Player.X + 1;
                        break;
                    }
                case Direction.UpLeft:
                    {
                        y = Game.Player.Y - 1;
                        x = Game.Player.X - 1;
                        break;
                    }
                case Direction.DownLeft:
                    {
                        y = Game.Player.Y + 1;
                        x = Game.Player.X - 1;
                        break;
                    }
                case Direction.DownRight:
                    {
                        y = Game.Player.Y + 1;
                        x = Game.Player.X + 1;
                        break;
                    }
                case Direction.Center:
                    {
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }
            Monster monster = Game.DungeonMap.GetMonsterAt(x, y);
            if(monster != null)
            {
                Attack(Game.Player, monster);
                return true;
            }
            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
            {
                return true;
            }

            return false;
        }
        public void Attack(Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);

            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);
            if (!string.IsNullOrWhiteSpace(attackMessage.ToString()))
            {
                Game.MessageLog.Add(attackMessage.ToString());
            }
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                Game.MessageLog.Add(defenseMessage.ToString());
            }

            int damage = hits - blocks;

            ResolveDamage(defender, damage, attacker);
        }

        // The attacker rolls based on his stats to see if he gets any hits
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            int hits = 0;            
            // Roll a number of 100-sided dice equal to the Attack value of the attacking actor
            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            // Look at the face value of each single die that was rolled
            foreach (TermResult termResult in attackResult.Results)
            {
                // Compare the value to 100 minus the attack chance and add a hit if it's greater
                if (termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            return hits;
        }

        // The defender rolls based on his stats to see if he blocks any of the hits from the attacker
        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if (hits > 0)
            {
                // Roll a number of 100-sided dice equal to the Defense value of the defendering actor
                DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult defenseRoll = defenseDice.Roll();

                // Look at the face value of each single die that was rolled
                foreach (TermResult termResult in defenseRoll.Results)
                {
                    // Compare the value to 100 minus the defense chance and add a block if it's greater
                    if (termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }
            }
            return blocks;
        }

        // Apply any damage that wasn't blocked to the defender
        private static void ResolveDamage(Actor defender, int damage, Actor attacker)
        {
            if (damage > 0)
            {
                defender.Health = defender.Health - damage;
                if (defender != Game.Player && attacker == Game.Player)
                    Game.MessageLog.Add($"You hit the {defender.Name}");
                else if(defender == Game.Player)
                    Game.MessageLog.Add($"The {attacker.Name} hits you");

                if (defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                if (defender != Game.Player && attacker == Game.Player)
                    Game.MessageLog.Add($"You miss the {defender.Name}");
                else if(defender == Game.Player)
                    Game.MessageLog.Add($"The {attacker.Name} misses you");
            }
        }

        // Remove the defender from the map and add some messages upon death.
        private static void ResolveDeath(Actor defender)
        {
            if (defender is Player)
            {
                
            }
            else if (defender is Monster)
            {
                Game.MessageLog.Add($"You kill the {defender.Name}");
                Game.DungeonMap.RemoveMonster((Monster)defender);
            }
        }

        public bool IsPlayerTurn { get; set; }

        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        public void ActivateMonsters()
        {
            ISchedulable scheduleable = Game.SchedulingSystem.Get();
            if (scheduleable is Player)
            {
                IsPlayerTurn = true;
                Game.SchedulingSystem.Add(Game.Player);
            }
            else
            {
                Monster monster = scheduleable as Monster;

                if (monster != null)
                {
                    monster.PerformAction(this);
                    Game.SchedulingSystem.Add(monster);
                }

                ActivateMonsters();
            }
        }

        public void MoveMonster(Monster monster, Cell cell)
        {
            if (!Game.DungeonMap.SetActorPosition(monster, cell.X, cell.Y))
            {
                if (Game.Player.X == cell.X && Game.Player.Y == cell.Y)
                {
                    Attack(monster, Game.Player);
                }
            }
        }
    }
}
