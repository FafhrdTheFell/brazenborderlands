using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands.Displays
{
    internal class InventoryDisplay : TabularDisplay
    {
        private static int LayerInventory = 100;

        //public InventoryDisplay() : this(2 * Consts.TermWidthBlocks / 3, 5 * Consts.TermHeightBlocks / 6, Consts.TermWidthBlocks / 6, Consts.TermHeightBlocks / 12) { }
        public InventoryDisplay() : this(90, 1 * Consts.TermHeightBlocks / 6, 35, 8) { }
        public InventoryDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset, 5, 5)
        {
            AddBorder = true;
        }

        public override void Draw()
        {
            if (!Dirty)
            {
                return;
            }
            if (GlobalDirty)
            {
                term.Composition(true);
                ColorDisplayLayer(LayerInventory, "grey", 0);
                //ColorDisplayLayer(LayerInventory, "DeepGreen", 1);
                //ColorDisplayLayer(LayerInventory, "Silver", 2);
                term.Composition(false);
                System.Console.WriteLine("bonk");
                term.Refresh();
                Program.gameLoop.WaitForKey();

            }
            int origLayer = termLayer();
            term.Layer(LayerInventory);
            base.Draw();
            for (int x = 0; x < CellsWidth; x+=2)
            {
                for (int y = 0; y < CellsHeight; y+=2)
                {
                    //term.Print(x + XOffset, y + YOffset, "X");
                }
            }
            term.Print(XOffset, YOffset, TileFinder.AssembledTile(5,15, "white"));
            term.Print(XOffset + 1, YOffset + 1, TileFinder.AssembledTile(5, 15, "white"));
            term.Print(XOffset + CellsWidth, YOffset + CellsHeight, TileFinder.AssembledTile(5, 15, "white"));
            term.Print(XOffset + CellsWidth - 1, YOffset + CellsHeight - 1, TileFinder.AssembledTile(5, 15, "white"));
            term.Print(XOffset, YOffset+5, TileFinder.AssembledTile(5, 15, "white"));
            term.Print(XOffset+1, YOffset + 5, TileFinder.AssembledTile(5, 15, "white"));
            term.Print(XOffset+CellsWidth, YOffset + 5, TileFinder.AssembledTile(5, 15, "white"));
            term.Print(XOffset + CellsWidth-1, YOffset + 5, TileFinder.AssembledTile(5, 15, "white"));
            term.Layer(origLayer);
            Dirty = false;
            GlobalDirty = false;
        }

        public void Hide()
        {
            ClearDisplayLayer(LayerInventory);
            ClearDisplayLayer(LayerInventory+1);
        }
    }
}
