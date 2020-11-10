using System;

namespace GameHost1.Universes.Evance.Milestone3
{
    public class TimeSettings
    {
        /// <summary>
        /// 世界最後の日
        /// </summary>
        public TimeSpan SekaiSaigoNoHi { get; set; }

        /// <summary>
        /// 第一次行動的延遲時間 (msec)
        /// </summary>
        public int StartDelay { get; set; }

        /// <summary>
        /// 每次行動的間隔時間 (msec)
        /// </summary>
        public int Interval { get; set; }

        public int EventHandlersCount { get; set; } = 1;
    }
}
