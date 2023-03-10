using RogueSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using term = BearLib.Terminal;


namespace brazenborderlands
{
    // function naming convention: a Draw function calls term. Print one or more times to draw 
    // Glyphs. A Glyph is a string defining a single logical tile (i.e., a 16x24 tile).
    internal class LocationDisplay : BorderedDisplay, IDisplayWindow
    {
        private static int LayerFurnishings = 2;
        private static int LayerItems = 5;
        private static int LayerActors = 8;
        private static int LayerInfoStats = 10;

        public Location location { get => Program.location; }

        private int ViewportAdjustment;
        private int ViewpointXBorder;
        private int ViewpointYBorder;
        private bool AutoAdjustFocus = true;
        private bool InfoStats = true;
        private bool ActorStats = false;

        private int _viewportMinX;
        private int _viewportMinY;

        private int XScale;
        private int YScale;

        public int ViewportMinX
        {
            get => _viewportMinX;
            set
            {
                if (_viewportMinX!= value) { Dirty = true;  _viewportMinX = value;}
            }
        }
        public int ViewportMinY
        {
            get => _viewportMinY;
            set
            {
                if (_viewportMinY != value) { Dirty = true;   _viewportMinY = value; }
            }
        }


        // maximum x cell shown in viewport
        public int ViewportMaxX { get { return ViewportMinX + TilesWidth; } }
        // maximum y cell shown in viewport
        public int ViewportMaxY { get { return ViewportMinY + TilesHeight; } }

        private FieldOfView OldFOV;

        public List<ICell> HighlightCells { get; set; }

       public LocationDisplay(int cellsWidth, int cellsHeight, int cellsXOffset, int cellsYOffset, Location startlocation) :
            base(cellsWidth, cellsHeight, cellsXOffset, cellsYOffset) {
            AddBorder = false;
            XOffset = cellsXOffset;
            YOffset = cellsYOffset;
            CellsWidth = cellsWidth;
            CellsHeight = cellsHeight;

            ViewportMinX = 0;
            ViewportMinY = 0;
            XScale = Consts.XScaleGlyphs;
            YScale = Consts.YScaleGlyphs;
            TilesWidth = cellsWidth / XScale;
            TilesHeight = cellsHeight / YScale;
            ViewportAdjustment = (TilesHeight + TilesWidth) / 5;
            ViewpointXBorder = TilesWidth / 8;
            ViewpointYBorder = TilesHeight / 8;

            HighlightCells = new List<ICell>();

            Dirty = true;
            GlobalDirty = false;
        }

        public override void Draw()
        {
            if (OldFOV==null)
            {
                OldFOV = location.FOV.Clone();
            }
            if (AutoAdjustFocus && Dirty)
            {
                AdjustFocusPlayer();
            }
            if (GlobalDirty)
            {
                term.Clear();
                DrawGlobal();
                GlobalDirty = false;
            }
            if (!Dirty)
            {
                return;
            }

            int minCellX = Math.Max(0, location.PlayerX-location.FOVRadius-3);
            int minCellY = Math.Max(0, location.PlayerY-location.FOVRadius-3);
            int maxCellX = Math.Min(location.Map.Width, location.PlayerX+location.FOVRadius+3);
            int maxCellY = Math.Min(location.Map.Height, location.PlayerY+location.FOVRadius+3);

            // clear layers for tiles of dynamic stuff (Actors move, Items get dropped / picked up)
            ClearLayer(LayerActors, minCellX, minCellY, maxCellX, maxCellY);
            ClearLayer(LayerItems, minCellX, minCellY, maxCellX, maxCellY);

            term.Layer(0);
            for (int x = minCellX; x < maxCellX; x++)
            {
                for (int y = minCellY; y < maxCellY; y++)
                {
                    if (location.FOV.IsInFov(x, y) || OldFOV.IsInFov(x, y))
                    {
                        bool highlight = HighlightCells.Exists(c => c.X == x && c.Y == y);
                        location.Glyphs[x,y].DrawAt(DisplayX(x),DisplayY(y), location.FOV.IsInFov(x, y), highlight);
                    }
                }
            }
            DrawFurniture();
            DrawItems();

            // drawing routines that rely on being able to determine when an object leaves
            // FOV must be above the following
            OldFOV = location.FOV.Clone();

            DrawActors();

            if (AddBorder)
            {
                DrawBorder();
            }

            if (InfoStats)
            {
                DrawInfoStats();
            }

            term.Refresh();
            Dirty = false;
        }

        private void ClearLayer(int layer, int xMin, int yMin, int xMax, int yMax)
        {
            int currentLayer = termLayer();
            term.Layer(layer);
            term.ClearArea(DisplayX(xMin), DisplayY(yMin), DisplayX(xMax) - DisplayX(xMin), DisplayY(yMax) - DisplayY(yMin));
            term.Layer(currentLayer);
        }

        private void DrawInfoStats()
        {
            int currentLayer = termLayer();
            term.Layer(LayerInfoStats);
            string s = Statline("location");
            term.Print(DisplayX(ViewportMaxX) - 12, DisplayY(ViewportMaxY) - 2, "           ");
            if (AddBorder)
            {
                term.Layer(0);
                term.Print(DisplayX(ViewportMaxX) - 12, DisplayY(ViewportMaxY) - 2, "           ");
                term.Layer(LayerInfoStats);
            }
            term.Print(DisplayX(ViewportMaxX) - s.Length - 2, DisplayY(ViewportMaxY) - 2, ColoredString(s, "red", "dark"));
            term.Layer(currentLayer);
        }

        private void DrawActors()
        {
            int currentLayer = termLayer(); 
            term.Layer(LayerActors);
            foreach (Actor a in location.Actors())
            {
                if (location.FOV.IsInFov(a.x, a.y))
                {
                    bool highlight = HighlightCells.Exists(c => c.X == a.x && c.Y == a.y);
                    a.Glyph.DrawAt(DisplayX(a.x), DisplayY(a.y), true, highlight);
                }
            }
            if (ActorStats)
            {
                foreach (Actor a in location.Actors())
                {
                    if (location.FOV.IsInFov(a.x, a.y))
                    {
                        term.Print(DisplayX(a.x)-1, DisplayY(a.y) + 2, ColoredString(a.Health, "red", "darker"));
                    }
                }
            }
            term.Layer(currentLayer);
        }

        // Furniture = stairs and furnishings
        private void DrawFurniture()
        {
            int currentLayer = termLayer();
            term.Layer(LayerFurnishings+1);
            term.Layer(LayerFurnishings);
            foreach (Furnishing f in location.Furniture())
            {
                if (location.Map.GetCell(f.x, f.y).IsExplored)
                {
                    bool highlight = HighlightCells.Exists(c => c.X == f.x && c.Y == f.y);
                    f.Glyph.DrawAt(DisplayX(f.x), DisplayY(f.y), location.FOV.IsInFov(f.x, f.y), highlight);
                }
            }
            term.Layer(currentLayer);
        }
        private void DrawItems()
        {
            int currentLayer = termLayer();
            term.Layer(LayerItems);
            foreach (Item i in location.Items)
            {
                if (location.Map.GetCell(i.x, i.y).IsExplored)
                {
                    bool highlight = HighlightCells.Exists(c => c.X == i.x && c.Y == i.y);
                    i.Glyph.DrawAt(DisplayX(i.x), DisplayY(i.y), location.FOV.IsInFov(i.x, i.y), highlight);
                }
            }
            term.Layer(currentLayer);
        }
        private void DrawGlobal()
        {
            int currentLayer = termLayer();
            term.Layer(0);
            for (int x = ViewportMinX; x < ViewportMaxX; x++)
            {
                for (int y = ViewportMinY; y < ViewportMaxY; y++)
                {
                    ICell c = location.Map.GetCell(x, y);
                    if (!c.IsExplored)
                    { continue; }
                    bool highlight = HighlightCells.Exists(h => h.X == c.X && h.Y == c.X);
                    location.Glyphs[x, y].DrawAt(DisplayX(x), DisplayY(y), location.FOV.IsInFov(x, y), highlight);
                }
            }
            DrawFurniture();
            DrawItems();
            term.Layer(currentLayer);
        }
        public string Statline(string statType)
        {
            if (statType == "location")
            {
                return Program.location.OvermapLocation.Depth.ToString() + ": " + Program.player.x.ToString() + ", " + Program.player.y.ToString();
            }
            return "XXXXX";
        }

        public int DisplayX(int MapX)
        {
            return (MapX - ViewportMinX) * XScale + XOffset + (AddBorder ? 1 : 0);
        }

        public int DisplayY(int MapY)
        {
            return (MapY - ViewportMinY) * YScale + YOffset + (AddBorder ? 1 : 0);
        }

        // returns true if view adjusted
        public bool AdjustFocusPlayer()
        {
            bool adjusted = false;
            int vpxOrig = ViewportMinX;
            int vpyOrig = ViewportMinY;
            // first test is for being too few tiles away from border; second is for drawing
            // outside the display
            if ((location.PlayerX > ViewportMaxX - ViewpointXBorder) || 
                (DisplayX(location.PlayerX + location.FOVRadius) >= XOffset + CellsWidth))
            {
                ViewportMinX = Math.Min(ViewportMinX + ViewportAdjustment, location.Map.Width - TilesWidth);
            }
            if ((location.PlayerY > ViewportMaxY - ViewpointYBorder) ||
                (DisplayY(location.PlayerY + location.FOVRadius) >= YOffset + CellsHeight))
            {
                ViewportMinY = Math.Min(ViewportMinY + ViewportAdjustment, location.Map.Height - TilesHeight);
            }
            if ((location.PlayerX < ViewportMinX + ViewpointXBorder) ||
                (DisplayX(location.PlayerX - location.FOVRadius) <= XOffset + XScale + (AddBorder ? 1 : 0)))
            {
                ViewportMinX = Math.Max(ViewportMinX - ViewportAdjustment, 0);
            }
            if ((location.PlayerY < ViewportMinY + ViewpointYBorder) ||
                (DisplayY(location.PlayerY - location.FOVRadius) <= YOffset + YScale + (AddBorder ? 1 : 0)))
            {
                ViewportMinY = Math.Max(ViewportMinY - ViewportAdjustment, 0);
            }
            if (vpxOrig != ViewportMinX || vpyOrig != ViewportMinY)
            {
                GlobalDirty = true;
                adjusted = true;
            }
            return adjusted;
        }

        public bool CenterPlayer()
        {
            if (TilesWidth >= location.Map.Width)
            {
                ViewportMinX = 0;
            }
            else
            {
                ViewportMinX = Math.Min(location.Map.Width - TilesWidth, Math.Max(0, location.PlayerX - TilesWidth / 2));
            }
            
            if (TilesHeight >= location.Map.Height)
            {
                ViewportMinY = 0;
            }
            else
            {
                ViewportMinY = Math.Min(location.Map.Height - TilesHeight, Math.Max(0, location.PlayerY - TilesHeight / 2));
            }
            GlobalDirty = true;
            AdjustFocusPlayer();
            return true;
        }

        // coloredstring: Bearlibterm may not handle extra spaces well between color= and
        // tint or color
        private string ColoredString(string text, string color, string tint)
        {
            string tintstring = (tint == "" ? "" : tint + " ");
            return "[color=" + tintstring + color + "]" + text + "[/color]";
        }

    }
}
