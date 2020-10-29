using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1
{
    public class Life
    {
        // game rules lookup table
        private static bool[] _survival_rules = new bool[] { false, false, true, true, false, false, false, false, false };
        private static bool[] _reborn_rules = new bool[] { false, false, false, true, false, false, false, false, false };

        private readonly Sensibility _sensibility;

        public bool IsAlive { get; private set; }

        public Life(out Sensibility sensibility, bool alive)
        {
            this._sensibility = sensibility = new Sensibility(this);
            this.IsAlive = alive;
        }

        private bool TimePass()
        {
            if (this._sensibility == null) throw new InvalidOperationException("can not do this if this life is clone");

            int around_lifes_count = 0;
            foreach (Life l in this._sensibility.SeeAround()) if (l != null && l.IsAlive) around_lifes_count++;

            bool result = false;
            if (this.IsAlive)
            {
                result = _survival_rules[around_lifes_count];
            }
            else
            {
                result = _reborn_rules[around_lifes_count];
            }

            return this.IsAlive = result;
        }

        public class Sensibility
        {
            private World _reality;
            private Life _itself;
            private (int x, int y) _position;

            private Func<Life[,]> _visibility = (() => { throw new InvalidOperationException("not initialized."); });

            public Sensibility(Life itself)
            {
                this._itself = itself;
            }

            public void InitWorldSide(World reality, (int x, int y) position, Func<Life[,]> visibility)
            {
                this._reality = reality;
                this._position = position;
                this._visibility = visibility;
            }

            public Life[,] SeeAround()
            {
                if (this._reality == null) throw new InvalidOperationException("world not initialized.");
                return this._visibility();
            }

            public bool TimePass()
            {
                if (this._reality == null) throw new InvalidOperationException("world not initialized.");
                return this._itself.TimePass();
            }

            public Life Itself
            {
                get
                {
                    return this._itself;
                }
            }

            public Life TakeSnapshot()
            {
                return new Life(out var _sense, this._itself.IsAlive);
            }
        }
    }
}
