using System;

namespace GameHost1.Universes.Evance.Milestone3
{
    public interface ITimeElapsingEvent
    {
        public event EventHandler<TimeEventArgs> Ready;
        public event EventHandler<TimeEventArgs> Elapsing;
        public event EventHandler<TimeEventArgs> Elapsed;
    }
}
