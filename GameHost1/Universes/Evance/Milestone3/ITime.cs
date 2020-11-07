namespace GameHost1.Universes.Evance.Milestone3
{
    public interface ITime : ITimeReadOnly
    {
        public void Start();

        public void Stop();

        public void ElapseOnce();
    }
}
