using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance.Milestone2
{
    public class LifeFactory: ILifeFactory
    {
        public ILife Generate(LifeSettings lifeSettings)
        {
            return new Life(lifeSettings);
        }
    }
}
