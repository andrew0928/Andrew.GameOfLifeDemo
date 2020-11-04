using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameHost1
{


    public class Life : ILife, IRunningObject
    {
        // game rules lookup table
        private static bool[] _survival_rules = new bool[] { false, false, true, true, false, false, false, false, false };
        private static bool[] _reborn_rules = new bool[] { false, false, false, true, false, false, false, false, false };

        private readonly Sensibility _sensibility;
        private static int _newid_seed = 0;

        public readonly int ID;
        public readonly int Frame;
       
        public bool IsAlive { get; private set; }

        private int _time_passed = 0;
        private int _generation = 0;

        // for snapshot use only
        private Life(Life item)
        {
            this.ID = item.ID;
            this._sensibility = null;
            this.IsAlive = item.IsAlive;
            this.Frame = 0;

            this._time_passed = 0;
        }

        public Life(out Sensibility sensibility, bool alive, int frame, int start_frames = 0)
        {
            this.ID = Interlocked.Increment(ref _newid_seed);
            this._sensibility = sensibility = new Sensibility(this);
            this.IsAlive = alive;
            this.Frame = frame;

            this._time_passed = start_frames;
        }


        IEnumerable<int> IRunningObject.AsTimePass()
        {
            while (true)
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
                this._time_passed += this.Frame;
                this._generation++;

                yield return this._time_passed;
            }
        }

        public class Sensibility
        {
            //private World _reality;
            private ILifeVision _vision;

            public readonly Life Itself;
            private (int x, int y) _position;

            //private Func<Life[,]> _visibility = (() => { throw new InvalidOperationException("not initialized."); });

            public Sensibility(Life itself)
            {
                this.Itself = itself;
            }

            public void InitWorldSide(ILifeVision reality, (int x, int y) position/*, Func<Life[,]> visibility*/)
            {
                this._vision = reality;
                this._position = position;
                //this._visibility = visibility;
            }

            public Life[,] SeeAround()
            {
                if (this._vision == null) throw new InvalidOperationException("world not initialized.");
                //return this._visibility();
                return this._vision.SeeAround(this._position.x, this._position.y);
            }

            public Life TakeSnapshot()
            {
                return new Life(this.Itself);
            }
        }
    }

}
