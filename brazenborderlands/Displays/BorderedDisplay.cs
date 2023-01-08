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
        public bool AddBorder { get; set; }

        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int TilesHeight { get; set; }
        public int TilesWidth { get; set; }

        public int CellsWidth { get; set; }
        public int CellsHeight { get; set; }

        public int BorderSpaces { get; set; }

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

        private string GlyphBorderNECorner = TileFinder.AssembledTile(36, 1, "brown");
        private string GlyphBorderNWCorner = TileFinder.AssembledTile(36, 2, "brown");
        private string GlyphBorderSECorner = TileFinder.AssembledTile(36, 3, "brown");
        private string GlyphBorderSWCorner = TileFinder.AssembledTile(36, 4, "brown");
        private string GlyphBorderHorizontal = TileFinder.AssembledTile(36, 5, "brown");
        private string GlyphBorderVertical = TileFinder.AssembledTile(36, 6, "brown");
        private int BorderGlyphWidthBlocks = Consts.XScaleGlyphs;
        private int BorderGlyphHeightBlocks = Consts.YScaleGlyphs;
        private int SolidGlyphWidthBlocks = Consts.XScaleGlyphs;
        private int SolidGlyphHeightBlocks = Consts.YScaleGlyphs;

        public BorderedDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset)
        {
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
        public void ColorDisplayLayer(int layer, string color, int borderSpaces)
        {
            int origLayer = termLayer();
            term.Layer(layer);
            for (int x = 0; x < CellsWidth; x+=1)
            {
                for (int y = 0; y < CellsHeight; y+=1)
                {
                    int adjx = Math.Min(CellsWidth - borderSpaces - SolidGlyphWidthBlocks, Math.Max(borderSpaces, x));
                    int adjy = Math.Min(CellsHeight - borderSpaces - SolidGlyphHeightBlocks, Math.Max(borderSpaces, y));
                    if (x > CellsWidth - 10) { System.Console.WriteLine(x.ToString() + " " + adjx.ToString()); }
                    term.Print(adjx + XOffset, adjy + YOffset, SolidGlyph(color));
                    if (x == 0)
                    {
                        System.Console.WriteLine("waited");
                        //Program.gameLoop.WaitForKey();
                        term.Refresh();
                    } 
                }
            }
            term.Layer(origLayer);
        }
        protected string SolidGlyph(string color)
        {
            return TileFinder.AssembledTile(37, 9, color);
        }
        protected int termLayer()
        {
            return term.State(term.TK_LAYER);
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
