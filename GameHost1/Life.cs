using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameHost1
{
    public class Life : ILife
    {
        // game rules lookup table
        private static bool[] _survival_rules = new bool[] { false, false, true, true, false, false, false, false, false };
        private static bool[] _reborn_rules = new bool[] { false, false, false, true, false, false, false, false, false };

        private readonly Sensibility _sensibility;
        private static int _seed = 0;

        public int ID { get; private set; }
        public int Frame { get; private set; }
       
        public bool IsAlive { get; private set; }

        private int _start_frame = 0;
        private int _generation = 0;

        // for snapshot use only
        private Life(Life item)
        {
            this.ID = item.ID;
            this._sensibility = null;
            this.IsAlive = item.IsAlive;
            this.Frame = 0;

            this._start_frame = 0;
        }

        public Life(out Sensibility sensibility, bool alive, int frame, int start_frames = 0)
        {
            this.ID = Interlocked.Increment(ref _seed);
            this._sensibility = sensibility = new Sensibility(this);
            this.IsAlive = alive;
            this.Frame = frame;

            this._start_frame = start_frames;
        }

        private int TimePass()
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

            this.IsAlive = result;

            return (this._generation++ == 0) ? (this._start_frame) : (this.Frame);
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

            public int TimePass()
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
                return new Life(this._itself);
            }
        }
    }

}
