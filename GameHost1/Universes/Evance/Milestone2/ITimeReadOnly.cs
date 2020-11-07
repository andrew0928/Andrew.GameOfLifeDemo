using System;

namespace GameHost1.Universes.Evance.Milestone2
{
    public interface ITimeReadOnly
    {
        public event EventHandler<TimeEventArgs> Ready;
        public event EventHandler<TimeEventArgs> Elapsing;
        public event EventHandler<TimeEventArgs> Elapsed;

        public int CurrentGeneration { get; }
    }
}
