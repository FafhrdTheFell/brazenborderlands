using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal interface IEmbodied
    {
        /// <summary>
        /// DrawingGlyph is '[U+E' + [hexadecimal representation of tile]  + ']'.
        /// </summary>
        public string DrawingGlyph { get; set; }
        public string DrawingColor { get; set; }
        int x { get; set; }
        int y { get; set; }
        bool IsWalkable { get; set; }
        bool IsTransparent { get; set; }

    }
}
