using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal interface IEmbodied
    {
        public Glyph Glyph { get; set; }
        int x { get; set; }
        int y { get; set; }
        bool IsWalkable { get; set; }
        bool IsTransparent { get; set; }

    }
}
