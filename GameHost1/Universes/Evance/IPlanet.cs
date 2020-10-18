using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public interface IPlanet
    {
        public int[] MaxCoordinates { get; }

        public bool TryPutLife(ILife life);

        public int GetAliveLivesCount(IEnumerable<int[]> coordinates);

    }
}
