namespace GameHost1.Universes.Evance.Milestone2
{
    public class LifeSettings
    {
        public ITimeReadOnly Time { get; set; }

        public IPlanetReadOnly Planet { get; set; }

        public int[] Coordinates { get; set; }

        public int EvolutionInterval { get; set; } = 1;

        public bool IsAlive { get; set; }
    }
}
