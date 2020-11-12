namespace GameHost1.Universes.Evance.Milestone3
{
    public class LifeSettings
    {
        public IPlanetReadOnly Planet { get; set; }

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
