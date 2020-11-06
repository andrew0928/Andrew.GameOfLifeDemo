using System.Threading;

namespace GameHost1.Universes.Evance.Milestone3
{
    public class LifeSettings
    {
        public ITimeReadOnly Time { get; set; }

        public IPlanetReadOnly Planet { get; set; }

        public SemaphoreSlim EvolvedSignal { get; set; }

        /// <summary>
        /// 初始化時， Life 是否存活
        /// </summary>
        public bool InitIsAlive { get; set; }

        /// <summary>
        /// 初始化時， Life 的座標
        /// </summary>
        public (int x, int y) InitCoordinates { get; set; }

        public TimeSettings TimeSettings { get; set; }
    }
}
