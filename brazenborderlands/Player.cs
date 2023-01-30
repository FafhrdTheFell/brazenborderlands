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
        public enum PlayerCommand
        {
            None,
            Move,
            Wait,
            Ascend,
            Descend,
            GetItems,
            Inventory,
            Look,
            MoveViewport,
            Next,
            Previous,
            Select,
            Exit,
            QuitGame
        }

        public string Class { get; set; }
        public string Species { get; set; }


        // PC's add half of Ego to HP max
        public override int SoakBase { get => 40 * Brawn() / 100; set { } }
        public override int HealthMax { 
            get => HealthBase + BrawnBase + EgoBase / 2; 
            set { HealthBase = (value - HealthBase - BrawnBase - EgoBase / 2); } 
        }

        public Player(int xcurrent, int ycurrent) : base(xcurrent, ycurrent, TileFinder.TileGridLookupUnicode(25, 2), "turquoise")
        {
            Glyph.Outline = true;
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
            KeyPress keyPress = new KeyPress();
            switch (keyPress.Command)
            {
                case PlayerCommand.QuitGame:
                    Program.active = false; acted = true;  break;
                case PlayerCommand.MoveViewport:
                    Program.locationDisplay.ViewportMinX += keyPress.moveDx;
                    Program.locationDisplay.ViewportMinY += keyPress.moveDy;
                    break;
                case PlayerCommand.Move:
                    acted = Program.location.Move(this, keyPress.moveDx, keyPress.moveDy); break;
                case PlayerCommand.Ascend:
                    acted = Program.location.Ascend(); break;
                case PlayerCommand.GetItems:
                    acted = Program.location.PickupItems(this); break;
                case PlayerCommand.Descend:
                    acted = Program.location.Descend(); break;
                case PlayerCommand.Inventory:
                    acted = Program.gameLoop.InventoryLoop(); break;
                case PlayerCommand.Look:
                    acted = Program.gameLoop.LookLoop(); break;

            }
            return acted;
        }
    }
}
