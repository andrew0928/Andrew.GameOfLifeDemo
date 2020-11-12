namespace GameHost1.Universes.Evance.Milestone3
{
    public interface ITime : ITimeElapsingEvent
    {
        public void Start();

        public void Stop();

        public void ElapseOnce();
    }
}
