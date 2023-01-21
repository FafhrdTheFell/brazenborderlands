using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands.Displays
{
    internal class InventoryDisplay : TabularDisplay
    {
        public enum InventoryMode
        {
            None,
            Drop,
            Equip,
            Use
        }

        InventoryMode _mode;
        public InventoryMode Mode {
            get => _mode;
            set
            {
                Dirty = true;
                _mode = value;
            }
        }
        public Player Player { get; set; }
        public InventoryDisplay() : this(5 * Consts.TermWidthBlocks / 6, 5 * Consts.TermHeightBlocks / 6, Consts.TermWidthBlocks / 12, 
            Consts.TermHeightBlocks / 12, Program.player) { }
        public InventoryDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset, Player player) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset, 6)
        {
            Player = player;
            Mode = InventoryMode.None;
            AddBorder = true;
            // spacing, letter, glyph, name, slot, description
            XTabs = new List<int> { 0,
                (int)((float)EffectiveCellsWidth()*0.05),
                (int)((float)EffectiveCellsWidth()*0.1),
                (int)((float)EffectiveCellsWidth()*0.15),
                (int)((float)EffectiveCellsWidth()*0.3),
                (int)((float)EffectiveCellsWidth()*0.5)
            };
            UpdateInventoryList();
            Player = player;
        }

        public override void Draw()
        {
            if (!Dirty)
            {
                return;
            }
            if (GlobalDirty)
            {
                term.BkColor("darker Gray");
                term.Clear();
                BorderColor = "Black";
                term.Color("Black");
            }
            UpdateInventoryList();
            base.Draw();
            for (int x = 0; x < CellsWidth; x+=2)
            {
                for (int y = 0; y < CellsHeight; y+=2)
                {
                    //term.Print(x + XOffset, y + YOffset, "X");
                }
            }
            Dirty = false;
            GlobalDirty = false;
        }
        protected void UpdateInventoryList()
        {
            InitContents();
            Contents[1, 0] = "INVENTORY";
            Contents[1, 1] = "d: Drop, e: Equip, u: Use";
            Contents[5, 0] = "Acc / Dam: ";
            Contents[5, 0] += Player.MeleeAttack().Accuracy(Player).ToString() + " / " +
                Player.MeleeAttack().Damage(Player).ToString();
            Contents[5, 0] += "            Def / Soak: ";
            Contents[5, 0] += Player.Defense().ToString() + " / " +
                Player.Soak().ToString() + "   ";
            if (Mode == InventoryMode.Drop)
            {
                Contents[1, 3] = "DROP?";
            }
            else if (Mode == InventoryMode.Equip) 
            {
                Contents[1, 3] = "EQUIP?";
            }
            else if (Mode == InventoryMode.Use)
            {
                Contents[1, 3] = "USE?";
            }
            else
            {
                Contents[1, 3] = "        ";
            }
            Contents[3, 3] = "ITEM";
            Contents[4, 3] = "EQUIPPED";
            Contents[5, 3] = "DESCRIPTION";
            int listStart = 4;
            for (int i = 0; i < Math.Min(Player.Inventory.NumItems(), YLines - listStart); i++)
            {
                Contents[1, i + listStart] = " ";
                if (Mode != InventoryMode.None)
                {
                    char c = (char)('a' + i);
                    if (Mode == InventoryMode.Equip && 
                        (Player.Inventory.Items[i] is Weapon ||
                        Player.Inventory.Items[i] is Armor))
                        Contents[1, i + listStart] = c.ToString();
                    if (Mode == InventoryMode.Use &&
                        Player.Inventory.Items[i] is Consumable)
                        Contents[1, i + listStart] = c.ToString();
                }
                Contents[2, i + listStart] = Player.Inventory.Items[i].DrawingGlyph;
                Contents[3, i + listStart] = Player.Inventory.Items[i].Name;
                Contents[4, i + listStart] = Player.Inventory.Items[i].IsEquipped ?
                    Enum.GetName(typeof(EquipmentSlot), Player.Inventory.Items[i].Slot) : "             ";
                Contents[5, i + listStart] = Player.Inventory.Items[i].Description;
            }
        }
        public int? ChosenItem(int input)
        {
            int choiceRow = input - 4; // BearLibTerm 'a' = integer 4
            if (choiceRow < 0 || choiceRow >= Player.Inventory.NumItems())
            {
                return null;
            }
            return choiceRow;
        }
        public bool EquipChosenItem(int input)
        {
            int? item = ChosenItem(input);
            if (item == null) return false;
            bool r = Program.location.Equip(Player, (int)item);
            if (r) Mode = InventoryMode.None;
            UpdateInventoryList();
            Dirty = true;
            return r;
        }
        public bool UseChosenItem(int input)
        {
            int? item = ChosenItem(input);
            if (item == null) return false;
            bool r = Program.location.UseItem(Player, (int)item);
            if (r) Mode = InventoryMode.None;
            UpdateInventoryList();
            Dirty = true;
            return r;
        }
        public bool DropChosenItem(int input)
        {
            int? item = ChosenItem(input);
            if (item == null) return false;
            bool r = Program.location.DropItem(Player, (int)item);
            if (r) Mode = InventoryMode.None;
            UpdateInventoryList();
            Dirty = true;
            GlobalDirty = true;
            return r;
        }
        public void Hide()
        {
            term.BkColor("Black");
            term.Color("white");
            Program.locationDisplay.GlobalDirty = true;
        }
    }
}
