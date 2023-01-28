using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal class Furnishing : IEmbodied
    {
        public Glyph Glyph { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public virtual bool IsWalkable { get; set; }
        public string Name { get; set; }
        public virtual bool IsInteractable { get; set; }
        public virtual bool IsTransparent { get => false; set { } }
        public Furnishing() 
        {
            Name = "gizmo";
            Glyph = new Glyph();
        }
    }
}
