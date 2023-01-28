using System;
using System.Collections.Generic;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands
{
    internal class TabularDisplay : BorderedDisplay
    {
        private int _tabularBorderSpaces;
        public int YLines { get; set; }
        public int XColumns { get; set; }
        public List<int> YTabs { get; set; }
        public List<int> XTabs { get; set; }
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
            YLines = rows;
            XColumns = columns;
            YTabs = rowTabs;
            XTabs = columnTabs;
            Contents = contents;
        }
        public TabularDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset,
        int columns) : base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset)
        {
            TabularBorderSpaces = 1;
            RowsAutoSpacing();
            XColumns = columns;
            ColumnsEqualSpacing();
            InitContents();
        }
        public TabularDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset,
            int rows, int columns) : base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset)
        {
            TabularBorderSpaces = 1;
            YLines = rows;
            XColumns = columns;
            ColumnsEqualSpacing();
            InitContents();
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
            for (int y = 0; y < YLines; y++) 
            {
                for (int x = 0; x < XColumns; x++)
                {
                    term.Print(DisplayX(XTabs[x]), DisplayY(YTabs[y]), Contents[x, y]);
                }
            }
        }
        protected void InitContents()
        {
            Contents = new string[XColumns, YLines];
            for (int x = 0; x < XColumns; x++)
            {
                for (int y = 0; y < YLines; y++)
                {
                    Contents[x, y] = new string("");
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
        public int GetXLocationForColumn(int column)
        {
            return DisplayX(XTabs[column]);
        }
        public int GetYLocationForRow(int row)
        {
            return DisplayY(YTabs[row]);
        }
        protected void RowsAutoSpacing()
        {
            YLines =  EffectiveCellsHeight() / Consts.YScaleText;
            YTabs = new List<int> { };
            for (int i = 0; i < EffectiveCellsHeight(); i += Consts.YScaleText)
            {
                YTabs.Add(i);
            }
        }
        protected void RowsEqualSpacing()
        {
            int rowSpacing = EffectiveCellsHeight() / (YLines - 1);
            YTabs = new List<int>() { };
            for (int y = 0; y < YLines; y++)
            {
                YTabs.Add(rowSpacing * y);
            }
        }
        protected void ColumnsEqualSpacing()
        {
            int columnSpacing = EffectiveCellsWidth() / (XColumns - 1);
            XTabs = new List<int>() {};
            for (int x = 0; x < XColumns; x++)
            {
                XTabs.Add(columnSpacing * x);
            }
        }
    }
}
