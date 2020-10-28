using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameHost1
{
    public class Life
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
}
