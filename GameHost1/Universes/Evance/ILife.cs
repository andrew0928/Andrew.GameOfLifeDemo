using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public interface ILife
    {
        public int[] Coordinates { get; }

        public bool IsAlive { get; }

        public bool Evolve();
    }
}
