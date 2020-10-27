using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1
{
    public class Life
    {
        private static bool[] _survival_rules = new bool[] { false, false, true, true, false, false, false, false, false };
        private static bool[] _reborn_rules = new bool[] { false, false, false, true, false, false, false, false, false };



        public bool IsAlive { get; private set; }

        private readonly World.LifeSensibility _sensibility;
        public Life(World.LifeSensibility sensibility, bool alive)
        {
            if (sensibility == null) throw new ArgumentNullException();
            this._sensibility = sensibility;
            this.IsAlive = alive;
        }

        public bool TimePass()
        {
            return this.IsAlive = this.CanSurvival();
        }

        private bool CanSurvival()
        {
            int around_lifes_count = 0;
            foreach (Life l in this._sensibility.SeeAround()) if (l != null && l.IsAlive) around_lifes_count++;

            if (this.IsAlive)
            {
                return _survival_rules[around_lifes_count];
            }
            else
            {
                return _reborn_rules[around_lifes_count];
            }
        }

        //private Life[,] SeeAround()
        //{
        //    return this._reality.SeeAround(this);
        //}

        public Life Snapshot
        {
            get
            {
                return new Life(this._sensibility, this.IsAlive);
            }
        }
    }
}
