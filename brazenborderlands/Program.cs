using System;
using BearLib;
using term = BearLib.Terminal;
using RogueSharp;
using RogueSharp.MapCreation;
using System.Collections.Generic;
using System.Drawing.Printing;
using brazenborderlands.Displays;

// needed to run dotnet add package System.Drawing.Common --version 6.0.0
// did that after nuget System.Drawing.Common

namespace brazenborderlands
{
    
    
    class Program
    {

        public static LocationDisplay locationDisplay;
        public static LogDisplay logDisplay;
        public static BorderedDisplay characterDisplay;
        public static InventoryDisplay inventoryDisplay;
        public static List<IDisplayWindow> Displays= new List<IDisplayWindow>();
        public static Location location;
        public static List<OvermapCoordinate> ExistingLocations = new List<OvermapCoordinate>();
        public static GameLoop gameLoop;
        public static bool active;
        public static Player player;

        static void Main(string[] args)
        {
            //int x = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            //int y = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            term.Open();
            string settings = "window: fullscreen=" + Consts.FullScreenDefault.ToString().ToLower() + ", size=" +
                Consts.TermWidthBlocks.ToString() + "x" + Consts.TermHeightBlocks.ToString() + ", cellsize=" +
                Consts.TermBlockWidthPx.ToString() + "x" + Consts.TermBlockHeightPx.ToString();
            settings += "; font: data/VeraMono.ttf, size=10, spacing=2x3";
            //settings += "; font: data/GeneralFailureRegular-Yj2a.ttf, size=10, use-box-drawing=true, spacing=3x4";
            
            //settings += "; log.level='debug'";
            //System.Console.WriteLine(settings);
            term.Set(settings);

            term.Set("U+E400: data/oryx_roguelike_16x24_trans.png, size=16x24, align=center, spacing=4x6");
            term.Set("U+E800: data/Items.png, size=16x24, align=center, spacing=4x6");
            term.Set("U+E1000: data/Monsters.png, size=16x24, align=center, spacing=4x6");
            term.Set("U+E1200: data/Terrain.png, size=16x24, align=center, spacing=4x6");

            // setting colors with hexadecimal does not work for some reason. Need to use RGB format.
            term.Set("palette.Brown = 129,74,19");
            term.Set("palette.Gold = 253,204,13");
            term.Set("palette.Silver = 192,192,192");
            term.Set("palette.Bronze = 156,82,33");
            term.Set("palette.Iron = 118,101,102");
            term.Set("palette.Black = 0,0,0");
            //term.Set("palette.YoungWood = #8D8752");
            term.Set("palette.YoungWood = 141,135,082");
            term.Set("palette.MediumGreen = 16,107,33");
            term.Set("palette.DeepGreen = 0,82,33");
            term.Set("palette.DarkGreen = 8,66,28");
            term.Set("palette.MediumBrown=102,51,0");
            term.Set("palette.DeepBrown=57,0,8");
            term.Set("palette.Slate=105,101,106");
            term.Set("palette.Ivory=243,236,212");
            term.Set("palette.Elfmetal=67,179,174");
            term.Set("palette.Cotton=230,227,223");
            term.Set("palette.RedLacquer=153,28,32");
            term.Set("palette.Red=204,0,26");


            player = new Player(0, 0);
            location = new Location(Consts.MapWidth, Consts.MapHeight, true, 1, 0, 0);
            locationDisplay = new LocationDisplay(Consts.LocationDisplayWidth, Consts.TermHeightBlocks, 0, 0, location);
            //characterDisplay = new BorderedDisplay(Consts.TermWidthBlocks - Consts.LocationDisplayWidth - 2, Consts.CharacterDisplayHeight,
            //   Consts.LocationDisplayWidth + 1, 0);
            characterDisplay = new PlayerDisplay(Consts.TermWidthBlocks - Consts.LocationDisplayWidth - 2, Consts.CharacterDisplayHeight,
                Consts.LocationDisplayWidth + 1, 0);
            logDisplay = new LogDisplay(Consts.TermWidthBlocks - Consts.LocationDisplayWidth - 2, Consts.TermHeightBlocks - Consts.CharacterDisplayHeight,
                Consts.LocationDisplayWidth + 1, Consts.CharacterDisplayHeight);
            inventoryDisplay = new InventoryDisplay();
            Displays.Remove(inventoryDisplay);

            locationDisplay.CenterPlayer();
            locationDisplay.Draw();
            logDisplay.Draw();
            characterDisplay.Draw();

            term.Refresh();


            active= true;
            gameLoop = new GameLoop();
            gameLoop.Run();

            //active= true;
            //while (running)
            //{
            //    int r = term.Read();
            //    switch (r)
            //    {
            //        case term.TK_ESCAPE:
            //            running = false; break;
            //        case term.TK_KP_4:
            //            mapLevel.MovePlayer(-1, 0); break;
            //        case term.TK_KP_6:
            //            mapLevel.MovePlayer(1, 0); break;
            //        case term.TK_KP_8:
            //            mapLevel.MovePlayer(0, -1); break;
            //        case term.TK_KP_2:
            //            mapLevel.MovePlayer(0, 1); break;
            //        //case term.TK_S:
            //        //    mapLevel.SaveMap("mapdata.json"); break;
            //        //case term.TK_L:
            //        //    mapLevel.LoadMap("mapdata.json"); break;
            //    }
            //    mapLevel.Draw();
            //    term.Refresh();
            //}

            BearLib.Terminal.Close();
        }

    }
}
