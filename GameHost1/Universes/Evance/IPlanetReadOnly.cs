using System.Collections.Generic;

namespace GameHost1.Universes.Evance
{
    public interface IPlanetReadOnly
    {
        public int[] MaxCoordinates { get; }

        public int GetAliveLivesCount(IEnumerable<int[]> coordinates);
    }
}
