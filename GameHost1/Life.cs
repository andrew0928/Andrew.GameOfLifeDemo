using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameHost1
{
    public class Life
    {
        // game rules lookup table
        private static bool[] _survival_rules = new bool[] { false, false, true, true, false, false, false, false, false };
        private static bool[] _reborn_rules = new bool[] { false, false, false, true, false, false, false, false, false };

        private readonly Sensibility _sensibility;

        private static int _seed = 0;

        public int ID { get; private set; }

        public int Frame { get; private set; }

        public bool IsAlive { get; private set; }


        private Life(Life item)
        {
            this.ID = item.ID;
            this._sensibility = null;
            this.IsAlive = item.IsAlive;
            this.Frame = 0;
        }

        public Life(out Sensibility sensibility, bool alive, int frame)
        {
            this.ID = Interlocked.Increment(ref _seed);
            this._sensibility = sensibility = new Sensibility(this);
            this.IsAlive = alive;
            this.Frame = frame;
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
            return this.Frame;
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



    /*
    public class _Life
    {
        private static bool[] _survival_rules = new bool[] { false, false, true, true, false, false, false, false, false };
        private static bool[] _reborn_rules = new bool[] { false, false, false, true, false, false, false, false, false };



        private static int _seed = 0;

        public int ID { get; private set; }

        public int Frame { get; private set; }

        public bool IsAlive { get; private set; }

        private readonly World.LifeSensibility _sensibility;
        public Life(World.LifeSensibility sensibility, bool alive, int frame)
        {
            this.ID = Interlocked.Increment(ref _seed);

            if (sensibility == null) throw new ArgumentNullException();
            this._sensibility = sensibility;
            this.IsAlive = alive;
            this.Frame = frame;
        }

        /// <summary>
        /// 觸發狀態轉換，同時傳回下次轉換時間 (單位: frames)
        /// </summary>
        /// <returns></returns>
        public int TimePass()
        {
            this.IsAlive = this.CanSurvival();
            return this.Frame;
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
                return this.MemberwiseClone() as Life; //new Life(this._sensibility, this.IsAlive, this.Frame);
            }
        }
    }
    */
}
