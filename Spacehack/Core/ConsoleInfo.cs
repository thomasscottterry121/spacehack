using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spacehack.Core;
using RLNET;
using RogueSharp;

namespace Spacehack.Core
{
    public class ConsoleInfo
    {
        public int width;
        public int height;
        public RLConsole console;
        public ConsoleInfo(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.console = new RLConsole(this.width, this.height);

        }
        public void blit(int x, int y, RLRootConsole root)
        {
            RLConsole.Blit(this.console, 0, 0, this.width, this.height, root, x, y);

        }

        public void setBackColor(RLColor color)
        {
            this.console.SetBackColor(0, 0, this.width, this.height, color);
        }
    }

    public class RootConsoleInfo : ConsoleInfo
    {
        new public RLRootConsole console;
        public RootConsoleInfo(int width, int height, string title = "Name") : base(width, height)
        {
            this.console = new RLRootConsole("terminal8x8.png", this.width, this.height, 8, 8, 1, title);
        }
    }
}
