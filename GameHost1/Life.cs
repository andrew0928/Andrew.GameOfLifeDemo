using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameHost1
{
    public class LifeSnapshot : ILife
    {
        public LifeSnapshot(bool alive, int generation)
        {
            this.IsAlive = alive;
            this.Generation = generation;
        }

        public bool IsAlive { get; private set; }
        public int Generation { get; private set; }
    }

    public class Life : ILife, IRunningObject
    {
        // game rules lookup table
        private static bool[] _survival_rules = new bool[] { false, false, true, true, false, false, false, false, false };
        private static bool[] _reborn_rules = new bool[] { false, false, false, true, false, false, false, false, false };

        private readonly Sensibility _sensibility;
        private static int _newid_seed = 0;

        public int ID { get; private set; }
        public readonly int Frame;
       
        public bool IsAlive { get; private set; }


        public Life(out Sensibility sensibility, bool alive, int frame, int start_frames = 0)
        {
            this.ID = Interlocked.Increment(ref _newid_seed);
            this._sensibility = sensibility = new Sensibility(this);
            this.IsAlive = alive;
            this.Frame = frame;

            this.Age = start_frames;
        }

        public int Age { get; private set; }
        public int Generation { get; private set; }

        IEnumerable<int> IRunningObject.AsTimePass()
        {
            while (true)
            {
                if (this._sensibility == null) throw new InvalidOperationException("can not do this if this life is clone");

                int around_lifes_count = 0;
                foreach (ILife l in this._sensibility.SeeAround()) if (l != null && l.IsAlive) around_lifes_count++;

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
                this.Age += this.Frame;
                this.Generation++;

                yield return this.Age;
            }
        }

        public class Sensibility
        {
            private ILifeVision _vision;

            public readonly Life Itself;
            private (int x, int y) _position;

            public Sensibility(Life itself)
            {
                this.Itself = itself;
            }

            public void InitWorldSide(ILifeVision reality, (int x, int y) position)
            {
                this._vision = reality;
                this._position = position;
            }

            public ILife[,] SeeAround()
            {
                if (this._vision == null) throw new InvalidOperationException("world not initialized.");
                return this._vision.SeeAround(this._position.x, this._position.y);
            }
        }
    }

}
