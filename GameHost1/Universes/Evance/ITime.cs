namespace GameHost1.Universes.Evance
{
    public interface ITime : ITimeReadOnly
    {
        public void Start();

        public void Stop();

        public void ElapseOnce();
    }
}
