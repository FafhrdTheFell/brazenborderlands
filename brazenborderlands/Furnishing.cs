using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal class Furnishing : IEmbodied
    {
        public string DrawingGlyph { get; set; }
        public string DrawingColor { get; set; }
        public string DrawingGlyphBack { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public virtual bool IsWalkable { get; set; }
        public string Name { get; set; }
        public virtual bool IsInteractable { get; set; }
        public virtual bool IsTransparent { get => false; set { } }
        public Furnishing() 
        {
            Name = "gizmo";
            DrawingGlyphBack = TileFinder.AssembledTile(37, 9, "black");
        }
    }
}
