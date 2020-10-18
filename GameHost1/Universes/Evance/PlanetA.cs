using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public class Planet : PlanetBase
    {
        public Planet(int x, int y) : base(x, y)
        {

        }

        public Planet(ILife[,] lives) : base(lives)
        {

        }


    }
}
