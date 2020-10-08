namespace GameHost1
{
    public interface ICell
    {
        public bool IsLive { get; }

        public bool Evolve();
    }
}
