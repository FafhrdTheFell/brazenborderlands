using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal interface IDisplayWindow
    {
        public bool AddBorder { get; set; }

        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int TilesHeight { get; set; }
        public int TilesWidth { get; set; }

        public int CellsWidth { get; set; }
        public int CellsHeight { get; set; }

        // GlobalDirty is true if screen needs to be cleared before draw
        // Dirty is true if parts of screen have been added to
        public bool Dirty { get; set; }
        public bool GlobalDirty { get; set; }

        public void Draw();
        public void SetGlobalDirtyTrue();
    }
}
