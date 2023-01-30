using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharp.MapCreation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace brazenborderlands
{
    internal class Location
    {
        public Map Map { get; set; }
        public Glyph[,] Glyphs { get; set; }
        public FieldOfView FOV { get; set; }
        public List<Monster> Monsters { get; set; }
        public List<Furnishing> Furnishings { get; set; }
        public List<Stair> Stairs { get; set; }
        public List<Item> Items { get; set; }
        public OvermapCoordinate OvermapLocation { get; set; }
        public LocationTrappings LocationTrappings { get; set; }


        public int FOVRadius = 10;

        public int PlayerX { get => Program.player.x; }
        public int PlayerY { get => Program.player.y; }

        public Location(int MapWidth, int MapHeight, bool randomizePlayerPosition, int depth, int west, int north)
        {
            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(MapWidth, MapHeight, 100, 7, 4);
            Map = Map.Create(mapCreationStrategy);
            FOV = new FieldOfView(Map);
            Monsters = new List<Monster>();
            Furnishings = new List<Furnishing>();
            Stairs = new List<Stair>();
            Items = new List<Item>();

            OvermapLocation = new OvermapCoordinate(depth, west, north);
            Program.ExistingLocations.Add(OvermapLocation);

            Glyphs = new Glyph[MapWidth, MapHeight];
            LocationTrappings = new LocationTrappings();
            InitWallsAndFloors();

            for (int i = 0; i < 8; i++)
            {
                Monster m = new Monster(1, Monster.MonsterKind.Gremlin, Monster.MonsterAttribute.Small, Monster.MonsterAttribute.Minion);
                RandomizePosition(m);
                Monsters.Add(m);
            }

            if (randomizePlayerPosition)
            {
                RandomizePositionNear(30, 20, Program.player);
                UpdatePlayerVision();
            }

            Stair stairsdown = new Stair(true);
            RandomizePositionNear(Program.player.x, Program.player.y, stairsdown);
            Stairs.Add(stairsdown);

            //Weapon sword = new Weapon(MeleeWeaponType.Broadsword, Material.IronWood);
            Weapon sword = Weapon.RandomWeapon(2);
            RandomizePositionNear(Program.player.x, Program.player.y, sword, 3);
            Items.Add(sword);

            Consumable pebble = Consumable.RandomConsumable(4);
            RandomizePositionNear(Program.player.x, Program.player.y, pebble, 3);
            Items.Add(pebble);

            Consumable pebble2 = Consumable.RandomConsumable(4);
            RandomizePositionNear(Program.player.x, Program.player.y, pebble2, 3);
            Items.Add(pebble2);

            Armor armor = Armor.RandomArmor(2);
            armor = (Armor)armor.Rebuild();
            RandomizePositionNear(Program.player.x, Program.player.y, armor, 4);
            Items.Add(armor);

            Monster monster = new Monster(1, Monster.MonsterKind.Raider, Monster.MonsterAttribute.Minion);
            RandomizePositionNear(Program.player.x, Program.player.y, monster, 12);
            Monsters.Add(monster);

            // for a reason I don't understand, Items and Stairs are invisible
            // until an Actor moves, unless I UpdatePlayerVision again
            UpdatePlayerVision();
        }

        public Location(Map map, List<Monster> monsters, Glyph[,] glyphs, List<Furnishing> furnishings, List<Stair> stairs, List<Item> items, OvermapCoordinate overmapCoordinate)
        {
            Map = map;
            Monsters = monsters;
            Glyphs = glyphs;
            Furnishings = furnishings;
            Stairs = stairs;
            Items = items;
            OvermapLocation = overmapCoordinate;
            FOV = new FieldOfView(Map);
        }

        public void RandomizePosition(IEmbodied embodied)
        {
            Cell c = RandomEmptyCell(0, 0, Map.Width - 1, Map.Height - 1);
            embodied.x = c.X;
            embodied.y = c.Y;
            Map.SetCellProperties(c.X, c.Y, c.IsTransparent, embodied.IsWalkable);
            return;
        }

        public void RandomizePositionNear(int x, int y, IEmbodied embodied)
        {
            RandomizePositionNear(x, y, embodied, 6);
        }
        public void RandomizePositionNear(int x, int y, IEmbodied embodied, int maxDelta)
        {
            Cell c = RandomEmptyCell(x - maxDelta, y - maxDelta, x + maxDelta, y + maxDelta);
            embodied.x = c.X;
            embodied.y = c.Y;
            Map.SetCellProperties(c.X, c.Y, c.IsTransparent, embodied.IsWalkable);
        }

        public Cell RandomEmptyCell(int xmin, int ymin, int xmax, int ymax)
        {
            xmin = Math.Max(xmin, 0);
            ymin = Math.Max(ymin, 0);
            xmax = Math.Min(xmax, Map.Width - 1);
            ymax = Math.Min(ymax, Map.Height - 1);
            int spanx = xmax - xmin;
            int spany = ymax - ymin;
            while (true)
            {
                int x = Dice.Roll(xmin.ToString() + " + 1d" + spanx.ToString());
                int y = Dice.Roll(ymin.ToString() + " + 1d" + spany.ToString());
                Cell c = (Cell)Map.GetCell(x, y);
                if (c.IsWalkable && GetActorAt(x,y) == null && GetItemAt(x, y) == null && GetStairsAt(x, y, true, true) == null)
                {
                    return c;
                }
            }
        }

        public bool Move(IEmbodied mover, int dx, int dy)
        {
            Cell dest = (Cell)Map.GetCell(mover.x + dx, mover.y + dy);
            Cell origin = (Cell)Map.GetCell(mover.x, mover.y);
            if (dest.IsWalkable)
            {
                if (!mover.IsWalkable)
                {
                    Map.SetCellProperties(mover.x, mover.y, origin.IsTransparent, true);
                    Map.SetCellProperties(dest.X, dest.Y, dest.IsTransparent, false);
                }
                mover.x += dx;
                mover.y += dy;
                UpdatePlayerVision();
                return true;
            }
            else if (GetActorAt(dest.X, dest.Y) != null)
            {
                Actor target = GetActorAt(dest.X, dest.Y);
                if ((mover is Monster && target is Player) ||
                    (mover is Player && target is Monster))
                {
                    Actor attacker = mover as Actor;
                    attacker.MeleeAttack().Attack(attacker, target);
                    //Combat.Attack((Actor)mover,target);
                    if (target is Monster && target.IsDead)
                    {
                        DeathOfMonster((Monster)target); 
                    }
                    return true;
                }
            }
            return false;
        }
        public void DeathOfMonster(Monster monster)
        {
            DropAllItems(monster);
            RemoveMonster(monster);
        }
        public bool Ascend()
        {
            Stair s = GetStairsAt(Program.player.x, Program.player.y, false, true);
            if (s == null)
            {
                return false;
            }
            ChangeLocation(OvermapLocation.Depth - 1, OvermapLocation.West, OvermapLocation.North, s);
            return true;
        }
        public bool Descend()
        {
            Stair s = GetStairsAt(Program.player.x, Program.player.y, true, false);
            if (s == null)
            {
                return false;
            }
            ChangeLocation(OvermapLocation.Depth + 1, OvermapLocation.West, OvermapLocation.North, s);
            return true;
        }
        public bool Equip(Player player)
        {
            if (player.Inventory.Items.Count == 0) { return false; }
            int randomItem = Dice.Roll("1d" + player.Inventory.Items.Count.ToString() + " -1");
            return Equip(player, randomItem);
        }
        public bool Equip(Player player, int item)
        {
            if (player.Inventory.Items.Count < item) { return false; }
            if (!player.Inventory.Items[item].IsEquippable) { return false; }
            bool currentlyEquipped = player.Inventory.Items[item].IsEquipped;
            if (currentlyEquipped)
            {
                player.Inventory.Unequip(item);
            }
            else
            {
                player.Inventory.Equip(item);
            }
            return true;
        }
        public bool UseItem(Actor possessor, int inventoryNum)
        {
            Item i = possessor.Inventory.Items[inventoryNum];
            if (!i.IsUsable) { return false; }
            bool success = possessor.Inventory.Use(inventoryNum, possessor);
            if (!success) return false;
            return true;
        }
        public bool DropItem(Actor possessor, int inventoryNum)
        {
            Item i = possessor.Inventory.Items[inventoryNum];
            bool success = possessor.Inventory.Remove(inventoryNum);
            if (!success) return false;
            i.x = possessor.x;
            i.y = possessor.y;
            Items.Add(i);
            return true;
        }
        public bool DropAllItems(Actor possessor)
        {
            if (possessor.Inventory.NumItems() == 0) return false;
            while (possessor.Inventory.NumItems() > 0)
            {
                DropItem(possessor, 0);
            }
            return true;
        }
        public bool PickupItems(Player player)
        {
            bool returnval = false;
            List<Item> pickedupItems = new List<Item>();
            IEnumerable<Item> itemsAt = Items.Where(i => i.x == player.x && i.y == player.y);
            foreach (Item item in itemsAt)
            {
                bool add = player.Inventory.Add(item);
                if (add)
                {
                    pickedupItems.Add(item);
                    returnval = true;
                }
            }
            Items.RemoveAll(i => pickedupItems.Contains(i));
            return returnval;
        }

        public void InitWallsAndFloors()
        {
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    ICell c = Map.GetCell(x, y);
                    ICell cSouth = Map.GetCell(x, Math.Min(y + 1, Map.Height - 1));
                    Glyphs[x, y] = LocationTrappings.GetGlyph(LocationTrappings.TileType.Floor);
                    if (!c.IsWalkable && !c.IsTransparent)
                    {
                        Glyphs[x, y] = LocationTrappings.GetGlyph(LocationTrappings.TileType.Wall);
                        if (cSouth.IsWalkable || cSouth.IsTransparent)
                        {
                            Glyphs[x, y] = LocationTrappings.GetGlyph(LocationTrappings.TileType.WallWithFloorSouth);
                        }
                    }
                }
            }
        }

        public void ChangeLocation(int depth, int west, int north, IPathBetween pathBetween)
        {
            OvermapCoordinate newOvermapLocation = Program.ExistingLocations.SingleOrDefault(d => d.Depth == depth && d.West == west && d.North == north);
            if (newOvermapLocation == null)
            {
                newOvermapLocation = new OvermapCoordinate(depth, west, north);
                Location newLocation = new Location(Consts.MapWidth, Consts.MapHeight, true, depth, west, north);
                Program.location = newLocation;

                Stair stairsup = new Stair(false);
                stairsup.x = Program.player.x;
                stairsup.y = Program.player.y;
                stairsup.TerminusX = pathBetween.EntranceX;
                stairsup.TerminusY = pathBetween.EntranceY;
                newLocation.Stairs.Add(stairsup);
                pathBetween.TerminusX = stairsup.x;
                pathBetween.TerminusY = stairsup.y;

                LocationSave curLocData = new LocationSave(this);
                curLocData.SaveToFile(OvermapLocation.SaveFilePrefix() + "data.json");
            }
            else
            {
                LocationSave curLocData = new LocationSave(this);
                curLocData.SaveToFile(OvermapLocation.SaveFilePrefix() + "data.json");

                LocationSave newLocData = new LocationSave(newOvermapLocation.SaveFilePrefix() + "data.json");

                Map map = newLocData.RestoredMap();
                Glyph[,] glyphs = newLocData.RestoredGlyphs();
                List<Item> items = newLocData.RestoredItems();
                List<Monster> monsters = newLocData.RestoredMonsters();
                List<Stair> stairs = newLocData.Stairs;
                List<Furnishing> furnishings = newLocData.Furnishings;
                Location location = new Location(map, monsters, glyphs, furnishings, stairs, items, newOvermapLocation);
                Program.location = location;
                Program.player.x = pathBetween.TerminusX;
                Program.player.y = pathBetween.TerminusY;
                location.UpdatePlayerVision();
            }

            Program.location.UpdatePlayerVision();
            Program.locationDisplay.CenterPlayer();
            Program.locationDisplay.GlobalDirty = true;
            Program.locationDisplay.Dirty = true;
        }

        public Stair GetStairsAt(int x, int y, bool stairdown, bool stairup)
        {
            Stair stair = Stairs.SingleOrDefault(d => d is Stair && d.x == x && d.y == y);
            if ((!stairdown && !stairup) || stair == null)
            {
                return stair;
            }
            else if (stair.StairDown == stairdown && stair.StairDown != stairup)
            {
                return stair;
            }
            return null;
        }

        public Monster GetMonsterAt(int x, int y)
        {
            return Monsters.SingleOrDefault(d => d.x == x && d.y == y);
        }

        public Item GetItemAt(int x, int y)
        {
            return Items.SingleOrDefault(d => d.x == x && d.y == y);
        }

        public Actor GetActorAt(int x, int y)
        {
            Actor a = Monsters.SingleOrDefault(d => d.x == x && d.y == y);
            if (a != null)
            {
                return a;
            }
            if (Program.player.x == x && Program.player.y == y)
            {
                return Program.player;
            }
            return null;
        }

        public void LookAt(int x, int y)
        {
            Monster m = GetMonsterAt(x, y);
            string mText = (m != null ? "a " + m.Name + " (health " + m.Health + ")." : "");
            Program.logDisplay.AppendEntry("You look at (" + x.ToString() + ", " + y.ToString() + ") and see ");
            Program.logDisplay.AppendEntry(mText != "" ? mText : "nothing.");
            Program.logDisplay.WriteBufferAsEntry();
        }
        public void RemoveMonster(Monster monster)
        {
            if (monster == null)
            {
                throw new ArgumentNullException("Null monster remove attempted.");
            }
            Cell loc = (Cell)Map.GetCell(monster.x, monster.y);
            Map.SetCellProperties(monster.x, monster.y, loc.IsTransparent, true);
            Monsters.Remove(monster);
        }

        public void UpdatePlayerVision()
        {
            foreach (Cell c in FOV.ComputeFov(Program.player.x, Program.player.y, FOVRadius, true))
            {
                Map.SetCellProperties(c.X, c.Y, c.IsTransparent, c.IsWalkable, true);
            }
        }

        public List<Actor> Actors()
        {
            Actor[] npcs = Monsters.ToArray();
            Actor[] player = { Program.player };
            //Actor[] all = npcs.Concat(player).ToArray();
            return npcs.Concat(player).ToList<Actor>();

        }

        public List<Furnishing> Furniture()
        {
            Furnishing[] furnishings = Furnishings.ToArray();
            Furnishing[] stairs = Stairs.ToArray();
            return furnishings.Concat(stairs).ToList<Furnishing>();
        }

    }

    internal class OvermapCoordinate
    {
        public int Depth { get; set; }
        public int West { get; set; }
        public int North { get; set; }
        public OvermapCoordinate(int depth, int west, int north) 
        { 
            Depth = depth;
            West = west;
            North = north;
        }

        public string SaveFilePrefix()
        {
            return "level-d" + Depth.ToString() + "-w" + West.ToString() + "-n" + North.ToString() + "-";
        }
    }

    internal class LocationTrappings
    {
        public enum LocationType
        {
            None,
            Classic,
            Overgrown
        }

        public enum TileType
        {
            Floor,
            Wall,
            WallWithFloorSouth
        }

        private Dictionary<TileType, List<Glyph>> TilesByType;
        public LocationType Type { get; set; }

        public LocationTrappings() : this(Helpers.RandomEnumValue<LocationType>()) { }

        public LocationTrappings(LocationType locationType) 
        {
            Type = locationType;
            TilesByType = new Dictionary<TileType, List<Glyph>>();
            foreach (TileType t in Enum.GetValues(typeof(TileType)))
            {
                TilesByType.Add(t, new List<Glyph>());
            }
            switch (locationType)
            {
                case LocationType.None:
                    Glyph g = new Glyph(TileFinder.TileGridLookupUnicode(5, 1, TileFinder.TileSheet.Terrain), "gray");
                    TilesByType[TileType.Wall].Add(new Glyph(TileFinder.TileGridLookupUnicode(5, 1, TileFinder.TileSheet.Terrain), "gray"));
                    g = new Glyph(TileFinder.TileGridLookupUnicode(4, 1, TileFinder.TileSheet.Terrain), "gray");
                    TilesByType[TileType.WallWithFloorSouth].Add(new Glyph(TileFinder.TileGridLookupUnicode(4, 1, TileFinder.TileSheet.Terrain), "gray"));
                    g = new Glyph(TileFinder.TileGridLookupUnicode(1, 14, TileFinder.TileSheet.Terrain), "Brown");
                    TilesByType[TileType.Floor].Add(new Glyph(TileFinder.TileGridLookupUnicode(1, 14, TileFinder.TileSheet.Terrain), "Brown"));
                    break;
                case LocationType.Overgrown:
                    TilesByType[TileType.Wall].Add(new Glyph(TileFinder.TileGridLookupUnicode(5, 1, TileFinder.TileSheet.Terrain), "DarkGreen"));
                    TilesByType[TileType.Floor].Add(new Glyph(TileFinder.TileGridLookupUnicode(1, 14, TileFinder.TileSheet.Terrain), "Brown"));
                    TilesByType[TileType.WallWithFloorSouth].Add(new Glyph(TileFinder.TileGridLookupUnicode(6, 9, TileFinder.TileSheet.Terrain), "DarkGreen",
                        TileFinder.TileGridLookupUnicode(5, 1, TileFinder.TileSheet.Terrain), "gray"));
                    TilesByType[TileType.WallWithFloorSouth].Add(new Glyph(TileFinder.TileGridLookupUnicode(6, 10, TileFinder.TileSheet.Terrain), "DarkGreen",
                        TileFinder.TileGridLookupUnicode(5, 1, TileFinder.TileSheet.Terrain), "gray"));
                    TilesByType[TileType.WallWithFloorSouth].Add(new Glyph(TileFinder.TileGridLookupUnicode(8, 2, TileFinder.TileSheet.Terrain), "DarkGreen",
                        TileFinder.TileGridLookupUnicode(5, 1, TileFinder.TileSheet.Terrain), "gray"));
                    break;
                case LocationType.Classic:
                    for (int i = 1; i <= 7; i++)
                    {
                        TilesByType[TileType.Floor].Add(new Glyph(TileFinder.TileGridLookupUnicode(1, i, TileFinder.TileSheet.Terrain), "Slate"));
                    }
                    for (int i = 1; i<= 3; i++)
                    {
                        TilesByType[TileType.WallWithFloorSouth].Add(new Glyph(TileFinder.TileGridLookupUnicode(6, i, TileFinder.TileSheet.Terrain), "Ivory"));
                    }
                    TilesByType[TileType.Wall].Add(new Glyph(TileFinder.TileGridLookupUnicode(5, 1, TileFinder.TileSheet.Terrain), "Ivory"));
                    break;
            }
        }

        public Glyph GetGlyph(TileType type)
        {
            List<Glyph> list = TilesByType[type];
            if (list.Count == 0) 
            {
                throw new Exception("Attempt to get glyph failed.");
            }
            int entry = Dice.Roll("1d" + list.Count.ToString() + " -1");
            return list[entry];
        }
    }
}
