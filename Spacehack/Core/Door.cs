using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spacehack.Interfaces;
using RogueSharp;
using Spacehack.Core;
using RLNET;

namespace Spacehack.Core
{
    public class Door : IDrawable
    {
        public Door()
        {
            Symbol = '+';
            Color = Colors.Text;
            BackgroundColor = Colors.FloorBackground;
        }
        public bool IsOpen { get; set; }

        public RLColor Color { get; set; }
        public RLColor BackgroundColor { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            Symbol = IsOpen ? '-' : '+';
            if (map.IsInFov(X, Y))
            {
                Color = Colors.Gold;
                BackgroundColor = Colors.FloorBackgroundFov;
            }
            else
            {
                Color = Colors.Text;
                BackgroundColor = Colors.FloorBackground;
            }

            console.Set(X, Y, Color, BackgroundColor, Symbol);
        }
    }
}
