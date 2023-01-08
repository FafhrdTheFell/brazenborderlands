using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace brazenborderlands
{
    public  class TileFinder
    {

        public enum TileSheet
        {
            None,
            Initial,
            Monsters,
            Items,
            Terrain
        }

        internal static Dictionary<TileSheet, int> SheetLocation = new Dictionary<TileSheet, int>()
        {
            {  TileSheet.Monsters, 4096},
            {  TileSheet.Items, 2048 },
            {  TileSheet.Initial, 1024 },
            {  TileSheet.Terrain, 4608 }
        };

        //internal static Dictionary<Monster.MonsterKind, string> MonsterTile = new Dictionary<Monster.MonsterKind, string>()
        //{
        //    { Monster.MonsterKind.Gremlin, TileGridLookupUnicode(11, 4, TileFinder.TileSheet.Monsters)  }
        //};

        internal static Dictionary<TileSheet, int> TilesPerRow = new Dictionary<TileSheet, int>()
        {
            {  TileSheet.Monsters, 19},
            {  TileSheet.Items, 20 }, // actually 19, but Bearlibterm miscalculates 
            {  TileSheet.Initial, 19 },
            {  TileSheet.Terrain, 16 }
        };
        //public static int TilesPerRow = 19;

        public static int HerosMonstersRow = 25;
        public static int WeaponsItemsRow = 4;
        public static int EnvironmentRow = 13;

        public static int TileGridLookup(int row, int column, TileSheet sheet)
        {
            return SheetLocation[sheet] + (column - 1) + (row - 1) * TilesPerRow[sheet];
        }

        public static int TileGridLookup(int row, int column)
        {
            // 1100 is shirt, shirt is row 7
            // 1024 is start
            //return 1023 + (row - 1) * 19 + column;
            //int i = 1023 + (row - 1) * 19 + column;
            //int j = TileGridLookup(row, column, TileSheet.Initial);
            //System.Console.WriteLine(i.ToString() +  " " + j.ToString());
            return TileGridLookup(row, column, TileSheet.Initial);
        }

        //public static string TileGridLookupHex(int row, int column)
        //{
        //    return TileGridLookup(row, column).ToString("X");
        //}

        public static string TileGridLookupUnicode(int row, int column)
        {
            return "[U+E" + TileGridLookup(row, column).ToString("X") + "]";
            //return "[U+E" + TileGridLookupHex(row, column) + "]";
        }

        public static string TileGridLookupUnicode(int row, int column, TileSheet sheet)
        {
            return "[U+E" + TileGridLookup(row, column, sheet).ToString("X") + "]";
            //return "[U+E" + TileGridLookupHex(row, column) + "]";
        }

        public static string AssembledTile(int row, int column, string color)
        {
            return "[color=" + color + "]" + TileGridLookupUnicode(row, column) + "[/color]";
            //return "[color=" + color + "][U+E" + TileGridLookupHex(row, column) + "][/color]";
        }
    }
}
