namespace GameHost1.Universes.Evance
{
    public interface ICell
    {
        public bool IsLive { get; }

        public bool Evolve();
    }
}
