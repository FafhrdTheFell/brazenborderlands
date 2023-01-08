using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal interface IPathBetween
    {
        public int TerminusX { get; set; }
        public int TerminusY { get; set;}
        public int EntranceX { get; set; }
        public int EntranceY { get; set; }
    }
}
