using System;

namespace GameHost1.Universes.Evance.Milestone3
{
    public class TimeEventArgs : EventArgs
    {
        public int CurrentGeneration { get; set; }

        public TimeSpan CurrentTime { get; set; }

        public TimeSpan LastTime { get; set; }

        public TimeSpan NextTime { get; set; }
    }
}
