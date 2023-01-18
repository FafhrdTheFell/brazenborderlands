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
            Equip
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
        public InventoryDisplay() : this(5 * Consts.TermWidthBlocks / 6, 5 * Consts.TermHeightBlocks / 6, Consts.TermWidthBlocks / 12, Consts.TermHeightBlocks / 12) { }
        public InventoryDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset, 6)
        {
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
            Contents[1, 1] = "d: Drop, e: Equip";
            Contents[5, 0] = "Acc / Dam: ";
            Contents[5, 0] += Program.player.MeleeAttack().Accuracy(Program.player).ToString() + " / " +
                Program.player.MeleeAttack().Damage(Program.player).ToString();
            Contents[5, 0] += "            Def / Soak: ";
            Contents[5, 0] += Program.player.Defense().ToString() + " / " +
                Program.player.Soak().ToString() + "   ";
            if (Mode == InventoryMode.Drop)
            {
                Contents[1, 3] = "DROP?";
            }
            else if (Mode == InventoryMode.Equip) 
            {
                Contents[1, 3] = "EQUIP?";
            }
            else
            {
                Contents[1, 3] = "        ";
            }
            Contents[3, 3] = "ITEM";
            Contents[4, 3] = "USE";
            Contents[5, 3] = "DESCRIPTION";
            int listStart = 4;
            for (int i = 0; i < Math.Min(Program.player.Inventory.NumItems(), YLines - listStart); i++)
            {
                char c = (char)('a' + i);
                if (Mode != InventoryMode.None)
                {
                    Contents[1, i + listStart] = c.ToString();
                }
                Contents[2, i + listStart] = Program.player.Inventory.Items[i].DrawingGlyph;
                Contents[3, i + listStart] = Program.player.Inventory.Items[i].Name;
                Contents[4, i + listStart] = Program.player.Inventory.Items[i].IsEquipped ?
                    Enum.GetName(typeof(EquipmentSlot), Program.player.Inventory.Items[i].Slot) : "             ";
                Contents[5, i + listStart] = Program.player.Inventory.Items[i].Description;
            }
        }
        public int? ChosenItem(int input)
        {
            int choiceRow = input - 4; // BearLibTerm 'a' = integer 4
            if (choiceRow < 0 || choiceRow >= Program.player.Inventory.NumItems())
            {
                return null;
            }
            return choiceRow;
        }
        public bool EquipChosenItem(int input)
        {
            int? item = ChosenItem(input);
            if (item == null) return false;
            bool r = Program.location.Equip(Program.player, (int)item);
            if (r) Mode = InventoryMode.None;
            UpdateInventoryList();
            Dirty = true;
            return r;
        }
        public bool DropChosenItem(int input)
        {
            int? item = ChosenItem(input);
            if (item == null) return false;
            bool r = Program.location.DropItem(Program.player, (int)item);
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
