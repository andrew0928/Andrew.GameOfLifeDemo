using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public class Life : ILife
    {
        private bool _areadyInit = false;
        private bool _isAlive = false;
        private bool _appearanceOnly = false;
        private int _generation = 0;
        private int _intervalFrame;
        private int _startAfterFrame;
        /// <summary>
        /// 還差多少 msec 就可以進化
        /// </summary>
        private int _leftTimeForEvolving;

        public bool IsAlive => _isAlive;

        public bool AppearanceOnly => _appearanceOnly;

        public int Generation => _generation;

        public Life()
        {
        }

        /// <summary>
        /// 建構花瓶 Life ，只能訪問 properties 。
        /// </summary>
        /// <param name="isAlive"></param>
        public Life(bool isAlive, int generation)
        {
            _appearanceOnly = true;
            _isAlive = isAlive;
            _generation = generation;
        }

        public bool Init(bool isAlive, int intervalFrame, int startAfterFrame)
        {
            if (_areadyInit || _appearanceOnly)
            {
                return false;
            }

            _areadyInit = true;

            _isAlive = isAlive;
            _intervalFrame = intervalFrame;
            _startAfterFrame = startAfterFrame;

            _leftTimeForEvolving = _startAfterFrame;

            return true;
        }


        public bool IsGoingOnToEvolve(TimeSpan time)
        {
            return time.TotalMilliseconds >= _leftTimeForEvolving;
        }

        public void TimePass()
        {
            // 環境一次週期期間， life 刷新一次跟多次是相同意義



        }

        public void Evolve()
        {

        }

        /// <summary>
        /// 取得花瓶 Life 。
        /// </summary>
        /// <param name="life"></param>
        /// <returns></returns>
        public static Life GetAppearanceOnlyLife(Life life)
        {
            return new Life(life.IsAlive, life.Generation);
        }
    }
}
