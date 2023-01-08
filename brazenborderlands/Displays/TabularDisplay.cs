using System;
using System.Collections.Generic;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands
{
    internal class TabularDisplay : BorderedDisplay
    {
        private int _tabularBorderSpaces;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<int> RowTabs { get; set; }
        public List<int> ColumnTabs { get; set; }
        public string[,] Contents { get; set; }
        public int TabularBorderSpaces 
        { 
            get => _tabularBorderSpaces + BorderSpaces; 
            set => _tabularBorderSpaces = value; 
        }
        public TabularDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset,
            int rows, int columns, List<int> rowTabs, List<int> columnTabs, string[,] contents) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset)
        {
            TabularBorderSpaces = 1;
            Rows = rows;
            Columns = columns;
            RowTabs = rowTabs;
            ColumnTabs = columnTabs;
            Contents = contents;
        }
        public TabularDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset,
            int rows, int columns) : base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset)
        {
            TabularBorderSpaces = 1;
            Rows = rows;
            Columns = columns;
            DefaultInit();
        }
        public TabularDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset)
        {
            TabularBorderSpaces = 1;
        }
        public override void Draw()
        {
            if (!Dirty)
            {
                return;
            }
            if (GlobalDirty && AddBorder)
            {
                DrawBorder();
            }
            for (int y = 0; y < Rows; y++) 
            {
                for (int x = 0; x < Columns; x++)
                {
                    term.Print(DisplayX(ColumnTabs[x]), DisplayY(RowTabs[y]), Contents[x, y]);
                }
            }
        }
        private void DefaultInit()
        {
            int rowSpacing = EffectiveCellsWidth() / (Rows-1);
            int columnSpacing = EffectiveCellsHeight() / (Columns - 1);
            Contents = new string[Rows, Columns];
            RowTabs = new List<int>() { 0 };
            ColumnTabs = new List<int>() { 0 };
            for (int x = 0; x < Rows; x++)
            {
                if (x > 0) ColumnTabs.Add(rowSpacing*x);
                for (int y = 0; y < Columns; y++)
                {
                    if (y > 0) RowTabs.Add(columnSpacing*y);
                    Contents[x, y] = new string(x.ToString() + y.ToString());
                    //Contents[x, y] = new string("");
                }
            }
        }
        protected int EffectiveCellsWidth()
        {
            return (CellsWidth - 2 * TabularBorderSpaces - 2 * Consts.XScaleGlyphs * (AddBorder ? 1 : 0) - 1);
        }
        protected int EffectiveCellsHeight()
        {
            return (CellsHeight - 2 * TabularBorderSpaces - 2 * Consts.YScaleGlyphs * (AddBorder ? 1 : 0) - 1);
        }
        private int DisplayX(int x)
        {
            return x + XOffset + TabularBorderSpaces + Consts.XScaleGlyphs * (AddBorder ? 1 : 0);
        }
        private int DisplayY(int y)
        {
            return y + YOffset + TabularBorderSpaces + Consts.YScaleGlyphs * (AddBorder ? 1 : 0);
        }
    }
}
