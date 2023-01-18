using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using term = BearLib.Terminal;

namespace brazenborderlands
{
    internal class BorderedDisplay : IDisplayWindow
    {
        private bool _globalDirty;
        private string _borderColor;
        public bool AddBorder { get; set; }

        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int TilesHeight { get; set; }
        public int TilesWidth { get; set; }

        public int CellsWidth { get; set; }
        public int CellsHeight { get; set; }

        public int BorderSpaces { get; set; }
        public string BorderColor { 
            get => _borderColor;
            set
            {
                SetBorderColor(value);
                _borderColor = value;
            } 
        }

        public bool Dirty { get; set; }
        public bool GlobalDirty
        {
            get => _globalDirty;
            set
            {
                if (value == false)
                {
                    _globalDirty = false;
                }
                else if (value == true && Program.Displays.Contains(this))
                {
                    foreach (IDisplayWindow display in Program.Displays)
                    {
                        display.Dirty = true;
                        display.SetGlobalDirtyTrue();
                    }
                }
                else
                {
                    _globalDirty = true;
                }
            }
        }

        private string GlyphBorderNECorner = TileFinder.AssembledTile(36, 1, "Gray");
        private string GlyphBorderNWCorner = TileFinder.AssembledTile(36, 2, "Gray");
        private string GlyphBorderSECorner = TileFinder.AssembledTile(36, 3, "Gray");
        private string GlyphBorderSWCorner = TileFinder.AssembledTile(36, 4, "Gray");
        private string GlyphBorderHorizontal = TileFinder.AssembledTile(36, 5, "Gray");
        private string GlyphBorderVertical = TileFinder.AssembledTile(36, 6, "Gray");
        private int BorderGlyphWidthBlocks = Consts.XScaleGlyphs;
        private int BorderGlyphHeightBlocks = Consts.YScaleGlyphs;
        private int SolidGlyphWidthBlocks = Consts.XScaleGlyphs;
        private int SolidGlyphHeightBlocks = Consts.YScaleGlyphs;

        public BorderedDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset)
        {
            BorderColor = "light gray";
            BorderSpaces = 1;
            AddBorder = true;
            XOffset = cellsXOffset;
            YOffset = cellsYOffset;
            CellsWidth = cellsWidth;
            CellsHeight = cellsHeight;
            Program.Displays.Add(this);
        }

        public virtual void Draw()
        {
            if (AddBorder && GlobalDirty)
            {
                DrawBorder();
            }
            GlobalDirty = false;
            Dirty = false;
        }

        protected void DrawBorder()
        {
            int cellXMin = XOffset + BorderSpaces;
            int cellYMin = YOffset + BorderSpaces;
            int cellXMax = XOffset + CellsWidth - BorderSpaces - BorderGlyphWidthBlocks;
            int cellYMax = YOffset + CellsHeight - BorderSpaces - BorderGlyphHeightBlocks;
            for (int x = cellXMin + 1; x < cellXMax; x++)
            {
                term.Print(x, cellYMin, GlyphBorderHorizontal);
                term.Print(x, cellYMax, GlyphBorderHorizontal);
            }
            for (int y = cellYMin + 1; y < cellYMax; y++)
            {
                term.Print(cellXMin, y, GlyphBorderVertical);
                term.Print(cellXMax, y, GlyphBorderVertical);
            }
            term.Print(cellXMin, cellYMin, GlyphBorderNWCorner);
            term.Print(cellXMin, cellYMax, GlyphBorderSWCorner);
            term.Print(cellXMax, cellYMin, GlyphBorderNECorner);
            term.Print(cellXMax, cellYMax, GlyphBorderSECorner);
        }

        public void ClearDisplayLayer(int layer)
        {
            int currentLayer = termLayer();
            term.Layer(layer);
            term.ClearArea(XOffset, YOffset, CellsWidth, CellsHeight);
            term.Layer(currentLayer);
        }
        protected string SolidGlyph(string color)
        {
            return TileFinder.AssembledTile(37, 9, color);
        }
        protected int termLayer()
        {
            return term.State(term.TK_LAYER);
        }
        protected void SetBorderColor(string color)
        {
            GlyphBorderNECorner = TileFinder.AssembledTile(36, 1, color);
            GlyphBorderNWCorner = TileFinder.AssembledTile(36, 2, color);
            GlyphBorderSECorner = TileFinder.AssembledTile(36, 3, color);
            GlyphBorderSWCorner = TileFinder.AssembledTile(36, 4, color);
            GlyphBorderHorizontal = TileFinder.AssembledTile(36, 5, color);
            GlyphBorderVertical = TileFinder.AssembledTile(36, 6, color);
    }
        // setting GlobalDirty=true with setter then sets
        // GlobalDirty=true for every display in Displays,
        // so causes infinite recursion if done with setter
        public void SetGlobalDirtyTrue()
        {
            _globalDirty= true;
        }
    }
}
