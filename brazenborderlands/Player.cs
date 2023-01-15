using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using term = BearLib.Terminal;

namespace brazenborderlands
{
    internal class Player : Actor
    {
        private int _icon;

        public string Class { get; set; }
        public string Species { get; set; }

        public int icon { 
            get => _icon; 
            set { _icon = value; UpdateGlyph(); }
        }

        // PC's add half of Ego to HP max
        public override int SoakBase { get => 40 * Brawn() / 100; set { } }
        public override int HealthMax { 
            get => HealthBase + BrawnBase + EgoBase / 2; 
            set { HealthBase = (value - HealthBase - BrawnBase - EgoBase / 2); } 
        }

        public Player(int xcurrent, int ycurrent) : base(xcurrent, ycurrent, "[U+E400]", "turquoise")
        {
            // TileFinder.TileGridLookupUnicode(8, 1, TileFinder.TileSheet.Items);
            icon = TileFinder.TileGridLookup(25, 2);
            //icon = TileFinder.TileGridLookup(1, 1, TileFinder.TileSheet.Items);
            InitByLevelNorm(1);
            Inventory = new Inventory();

            Name = "Boraxus";
            Species = "Elf";
            Class = "Warrior";
            Level = 1;
        }
        public override bool Act()
        {
            bool acted = false;
            int r = term.Read();
            // shift does not work with numpad on my laptop...
            bool shiftdown = (term.State(term.TK_SHIFT) == 1);
            switch (r)
            {
                case term.TK_ESCAPE:
                    Program.active = false; acted = true;  break;
                case term.TK_W:
                    Program.locationDisplay.ViewportMinY -= 5;  break;
                case term.TK_S:
                    Program.locationDisplay.ViewportMinY += 5; break;
                case term.TK_A:
                    Program.locationDisplay.ViewportMinX-= 5; break;
                case term.TK_D:
                    Program.locationDisplay.ViewportMinX+= 5; break;
                case term.TK_KP_1:
                    acted = Program.location.Move(this, -1, 1); break;
                case term.TK_KP_2:
                    acted = Program.location.Move(this, 0, 1); break;
                case term.TK_KP_3:
                    acted = Program.location.Move(this, 1, 1); break;
                case term.TK_KP_4:
                    acted = Program.location.Move(this, -1, 0); break;
                case term.TK_KP_5:
                    acted = true; break;
                case term.TK_KP_6:
                    acted = Program.location.Move(this, 1, 0); break;
                case term.TK_KP_7:
                    acted = Program.location.Move(this, -1, -1); break;
                case term.TK_KP_8:
                    acted = Program.location.Move(this, 0, -1); break;
                case term.TK_KP_9:
                    acted = Program.location.Move(this, 1, -1); break;
                //case term.TK_F1:
                //    Program.location.SaveMap("mapdata.json"); break;
                //case term.TK_F2:
                //    Program.location.LoadMap("mapdata.json"); break;
                //case term.TK_F3:
                //    Program.location.SaveNPCS("npcdata.json"); break;
                //case term.TK_F4:
                //    Program.location.LoadNPCS("npcdata.json"); break;
                case term.TK_I:
                    acted = Program.location.Equip(this); break;
                case term.TK_COMMA:
                    if (shiftdown)
                    {
                        acted = Program.location.Ascend();
                    }
                    else
                    {
                        System.Console.WriteLine("pickup");
                        acted = Program.location.PickupItems(this);
                    }
                    break;
                case term.TK_PERIOD:
                    if (shiftdown)
                    {
                        acted = Program.location.Descend();
                    }
                    break;

                case term.TK_F1:
                    icon++;
                    acted = true;
                    break;
                case term.TK_F2:
                    icon--;
                    acted = true;
                    break;
                case term.TK_F3:
                    Program.gameLoop.InventoryLoop(); break;

            }
            return acted;
        }
        //public override Weapon MeleeAttack()
        //{
        //    return Inventory.EquippedWeapon() ?? new Weapon(MeleeWeaponType.Fist);
        //}
        //public override int Defense()
        //{
        //    int def = Reflexes();
        //    foreach(Armor a in Inventory.EquippedArmors())
        //    {
        //        def += a.DefenseChange(this);
        //    }
        //    return def;
        //}
        //public override int Soak()
        //{
        //    int soak = Inventory.IsBodyArmorEquipped() ? 0 : 40 * Brawn() / 100;
        //    foreach (Armor a in Inventory.EquippedArmors())
        //    {
        //        soak += a.SoakChange(this);
        //    }
        //    return soak;
        //}
        private void UpdateGlyph()
        {
            string hex = icon.ToString("X");
            this.DrawingGlyph = "[U+E" + hex + "]";
        }
    }
}
