using System;

namespace GameHost1
{
    public class Clock
    {
        public int LapTimes { get; set; } = 0;
        public TimeSpan Elapsed { get; set; } = TimeSpan.Zero;

        public Clock() { }

        public void Lap()
        {
            this.LapTimes += ConfigProvider.MinimumFrame;
            this.Elapsed = this.Elapsed.Add(TimeSpan.FromMilliseconds(ConfigProvider.MinimumFrame));
        }
    }
}
