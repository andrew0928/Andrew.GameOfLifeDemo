using System.Collections.Generic;

namespace GameHost1.Universes.Evance.Milestone3
{
    public interface IPlanetReadOnly
    {
        public (int XAsis, int YAsis) MaxCoordinates { get; }

        public int GetAliveLivesCount(IEnumerable<(int X, int Y)> coordinates);

        public int GetAroundAliveLivesCount((int X, int Y) tartgetCoordinates);
    }
}
