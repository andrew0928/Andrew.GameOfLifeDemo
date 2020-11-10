using System;

namespace GameHost1
{
    public class Clock
    {
        public int LapTimes { get; set; } = 0;
        public TimeSpan Elapsed { get; set; } = TimeSpan.Zero;
        public Action Action { get; set; }

        public Clock() { }

        public Clock(Action action) 
        {
            this.Action = action;
        }

        public void Reset() 
        {
            this.LapTimes = 0;
            this.Elapsed = TimeSpan.Zero;
        }

        public void Lap()
        {
            this.LapTimes += ConfigProvider.MinimumFrame;
            this.Elapsed = this.Elapsed.Add(TimeSpan.FromMilliseconds(ConfigProvider.MinimumFrame));

            Action.Invoke();
        }
    }
}
