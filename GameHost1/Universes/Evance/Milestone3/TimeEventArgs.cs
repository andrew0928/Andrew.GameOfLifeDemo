using System;

namespace GameHost1.Universes.Evance.Milestone3
{
    public class TimeEventArgs : EventArgs
    {
        public TimeSpan CurrentTime { get; set; }

        public TimeSpan NextTime { get; set; }
    }
}
