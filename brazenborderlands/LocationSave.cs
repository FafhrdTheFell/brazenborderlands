using RogueSharp;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace brazenborderlands
{
    // this class is distantly based on MapSave in RogueSharp by Faron Bracy.
    // serializing Location to JSON does not work for two reasons: 2d arrays cannot
    // be serialized, and serialization of derived classes (i.e. Armor instead of Item)
    // loses the derived class (i.e., an Armor would be serialized as an Item if it
    // was stored in a List<Item>). To solve the first problem, LocationSave converts
    // 2d arrays into 1d arrays, which are serializable. To solve the second problem,
    // Items with derived classes are Rebuild'ed, which creates an object of the
    // correct class using Item.Template
    internal class LocationSave
    {
        /// <summary>
        /// Flags Enumeration of the possible properties for any Cell in the Map
        /// </summary>
        [Flags]
        public enum CellProperties
        {
            /// <summary>
            /// Not set
            /// </summary>
            None = 0,
            /// <summary>
            /// A character could normally walk across the Cell without difficulty
            /// </summary>
            Walkable = 1,
            /// <summary>
            /// There is a clear line-of-sight through this Cell
            /// </summary>
            Transparent = 2,
            IsExplored = 4
        }

        /// <summary>
        /// How many Cells wide the Map is
        /// </summary>
        public int Width
        {
            get; set;
        }

        /// <summary>
        /// How many Cells tall the Map is
        /// </summary>
        public int Height
        {
            get; set;
        }

        /// <summary>
        /// An array of the Flags Enumeration of CellProperties for each Cell in the Map.
        /// The index of the array corresponds to the location of the Cell within the Map using the formula: index = ( y * Width ) + x
        /// </summary>
        public CellProperties[] Cells
        {
            get; set;
        }

        public Glyph[] Glyphs { get; set; }
        public List<Monster> MonstersToRebuild { get; set; }
        public List<Furnishing> Furnishings { get; set; }
        public List<Stair> Stairs { get; set; }
        public List<Item> ItemsToRebuild { get; set; }

        /// <summary>
        /// Returns a MapState POCO which represents this Map and can be easily serialized
        /// Use Restore with the MapState to get back a full Map
        /// </summary>
        /// <returns>MapState POCO (Plain Old C# Object) which represents this Map and can be easily serialized</returns>
        public LocationSave() { }
        public LocationSave(Location location)
        {
            Map map = location.Map;
            Glyph[,] glyphs = location.Glyphs;
            Width = map.Width;
            Height = map.Height;
            Cells = new LocationSave.CellProperties[map.Width * map.Height];
            Glyphs = new Glyph[map.Width * map.Height];
            MonstersToRebuild = location.Monsters;
            Furnishings = location.Furnishings;
            Stairs = location.Stairs;
            ItemsToRebuild = location.Items;

            foreach (Cell cell in map.GetAllCells())
            {
                LocationSave.CellProperties cellProperties = LocationSave.CellProperties.None;
                if (cell.IsTransparent)
                {
                    cellProperties |= LocationSave.CellProperties.Transparent;
                }
                if (cell.IsWalkable)
                {
                    cellProperties |= LocationSave.CellProperties.Walkable;
                }
                if (cell.IsExplored)
                {
                    cellProperties |= LocationSave.CellProperties.IsExplored;
                }
                Cells[(cell.Y * map.Width) + cell.X] = cellProperties;
            }

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Glyphs[(y * map.Width) + x] = glyphs[x, y];
                }
            }
        }
        public LocationSave(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }
            string jsonString = File.ReadAllText(filename);
            LocationSave locData = JsonSerializer.Deserialize<LocationSave>(jsonString);
            Glyphs = locData.Glyphs;
            Width = locData.Width;
            Height = locData.Height;
            Cells = locData.Cells;
            MonstersToRebuild = locData.MonstersToRebuild;
            Furnishings = locData.Furnishings;
            Stairs = locData.Stairs;
            ItemsToRebuild = locData.ItemsToRebuild;

        }
        public void SaveToFile(string filename)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filename, jsonString);
        }

        /// <summary>
        /// Restore the state of this Map from the specified MapState
        /// </summary>
        /// <param name="state">MapState POCO (Plain Old C# Object) which represents this Map and can be easily serialized and deserialized</param>
        /// <exception cref="ArgumentNullException">Thrown on null map state</exception>
        public Map RestoredMap()
        {
            Map map = new Map(Width, Height);

            foreach (Cell cell in map.GetAllCells())
            {
                LocationSave.CellProperties cellProperties = Cells[(cell.Y * Width) + cell.X];

                bool IsTransparent = cellProperties.HasFlag(CellProperties.Transparent);
                bool IsWalkable = cellProperties.HasFlag(CellProperties.Walkable);
                bool IsExplored = cellProperties.HasFlag(CellProperties.IsExplored);

                map.SetCellProperties(cell.X, cell.Y, IsTransparent, IsWalkable, IsExplored);

            }

            return map;

        }

        public Glyph[,] RestoredGlyphs()
        {

            Glyph[,] gs = new Glyph[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    gs[x, y] = Glyphs[(y * Width) + x];
                }
            }
            return gs;

        }

        public List<Item> RestoredItems()
        {
            List<Item> items = new List<Item>();
            foreach (Item i in ItemsToRebuild)
            {
                items.Add(i.Rebuild());
            }
            return items;
        }
        public  List<Monster> RestoredMonsters()
        {
            List<Monster> monsters = new List<Monster>();
            foreach (Monster monster in MonstersToRebuild)
            {
                monster.Inventory.Rebuild();
                monsters.Add(monster);
            }
            return monsters;
        }
    }
}
