namespace GameHost1.Universes.Evance.Milestone2
{
    public interface ITime : ITimeReadOnly
    {
        public void Start();

        public void Stop();

        public void ElapseOnce();
    }
}
