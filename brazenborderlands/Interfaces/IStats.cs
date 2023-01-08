using System;
using System.Collections.Generic;
using System.Text;

namespace brazenborderlands
{
    internal interface IStats
    {
        public int BrawnBase { get; set; }
        public int ReflexesBase { get; set; }
        public int EgoBase { get; set; }
        public int HitpointBase { get; set; }
        public int Brawn();
        public int Reflexes();
        public int Ego();
        public int Defense();
        public int Soak();


    }
}
