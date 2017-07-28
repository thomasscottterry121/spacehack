using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RLNET;
using RogueSharp;
using System.Threading.Tasks;
using Spacehack.Core;
using RogueSharp.Random;
using Spacehack.Systems;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace Spacehack
{
    class Game
    {
        // Consoles to use for rendering
        private static RootConsoleInfo rootCon = new RootConsoleInfo(100, 70, "Spacehack");
        private static ConsoleInfo mapCon = new ConsoleInfo(80, 50);
        private static ConsoleInfo statCon = new ConsoleInfo(20, 70);
        private static ConsoleInfo messageCon = new ConsoleInfo(80, 10);
        private static ConsoleInfo itemCon = new ConsoleInfo(80, 10);
        private static MessageLog messageLog = new MessageLog();

        // the game objects
        public static DungeonMap DungeonMap { get; private set; }
        public static Player Player { get; set; }
        private static int mapLevel = 1;

        // input and message control
        public static CommandSystem CommandSystem { get; private set; }
        public static IRandom Random { get; private set; }
        public static MessageLog MessageLog { get { return messageLog; } private set { messageLog = value; } }
        public static uint Seed { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }

        static void Main(string[] args)
        {
            // generate a seed value from the clock
            Seed = (uint)DateTime.UtcNow.Ticks;
            MessageLog.Add($"level created with seed {Seed}");
            Random = new DotNetRandom((int)Seed);

            // setup the callbacks for the console
            rootCon.console.Update += RootConsole_Update;
            rootCon.console.Render += RootConsole_Render;
            // initialize input
            CommandSystem = new CommandSystem();
            SchedulingSystem = new SchedulingSystem();

            //generate the map
            MapGenerator MapGenerator = new MapGenerator(mapCon.width, mapCon.height, 30, 13, 7, mapLevel);
            DungeonMap = MapGenerator.CreateMap();

            // tell the console to start rendering
            rootCon.console.Run();
           
            
        }

        private static void RootConsole_Render(object sender, UpdateEventArgs e)
        {
            // draw everything to the correct consoles
            DungeonMap.Draw(mapCon.console, statCon.console);
            Player.Draw(mapCon.console, DungeonMap);
            Player.DrawStats(statCon.console);
            rootCon.console.Draw();
            MessageLog.Draw(messageCon.console);
            
            // copy the terminal buffers to the root window
            mapCon.blit(0, 10, rootCon.console);
            messageCon.blit(0, 0, rootCon.console);
            itemCon.blit(0, 60, rootCon.console);
            statCon.blit(80, 0, rootCon.console);
        }

        private static void RootConsole_Update(object sender, UpdateEventArgs e)
        {
            // check for a keypress
            RLKeyPress keyPress = rootCon.console.Keyboard.GetKeyPress();
            if (keyPress != null)
            {
                switch (keyPress.Key)
                {
                    case RLKey.Keypad9:
                        CommandSystem.MovePlayer(Direction.UpRight);
                        break;
                    case RLKey.Keypad8:
                        CommandSystem.MovePlayer(Direction.Up);
                        break;
                    case RLKey.Keypad7:
                        CommandSystem.MovePlayer(Direction.UpLeft);
                        break;
                    case RLKey.Keypad6:
                        CommandSystem.MovePlayer(Direction.Right);
                        break;
                    case RLKey.Keypad5:
                        CommandSystem.MovePlayer(Direction.Center);
                        break;
                    case RLKey.Keypad4:
                        CommandSystem.MovePlayer(Direction.Left);
                        break;
                    case RLKey.Keypad3:
                        CommandSystem.MovePlayer(Direction.DownRight);
                        break;
                    case RLKey.Keypad2:
                        CommandSystem.MovePlayer(Direction.Down);
                        break;
                    case RLKey.Keypad1:
                        CommandSystem.MovePlayer(Direction.DownLeft);
                        break;
                    case RLKey.Right:
                        if (DungeonMap.CanMoveDownToNextLevel())
                        {
                            MapGenerator mapGenerator = new MapGenerator(mapCon.width, mapCon.height, 20, 13, 7, mapLevel++);
                            DungeonMap = mapGenerator.CreateMap();
                            MessageLog = new MessageLog();
                            CommandSystem = new CommandSystem();                            
                        }
                        break;
                    case RLKey.Escape:
                        rootCon.console.Close();
                        break;
                }
                CommandSystem.EndPlayerTurn();
                CommandSystem.ActivateMonsters();

            }
        }
    }
}
