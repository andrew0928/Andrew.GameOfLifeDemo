using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public class LifeSettings
    {
        public int[] Coordinates { get; set; }

        public int EvolutionInterval { get; set; } = 1;

        public bool IsAlive { get; set; }
    }
}
