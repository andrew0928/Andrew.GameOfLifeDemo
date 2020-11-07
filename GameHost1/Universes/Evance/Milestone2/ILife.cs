namespace GameHost1.Universes.Evance.Milestone2
{
    public interface ILife
    {
        public int[] Coordinates { get; }

        public bool IsAlive { get; }

        public bool Evolve();
    }
}
