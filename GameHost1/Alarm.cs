using System;

namespace GameHost1
{
    public class Alarm
    {
        private int StartTime { get; set; }

        private bool IsStarted { get; set; } = false;

        private int Interval { get; set; }

        private int LapTimes { get; set; }

        private Action Action { get; set; }

        public Alarm(int startTime, int interval, Action action) 
        {
            this.StartTime = startTime;
            this.Interval = interval;
            this.Action = action;

            this.IsStarted = StartTime == 0;
        }

        public void Lap() 
        {
            LapTimes += 10;

            if (!IsStarted)
                Start();
            if (IsStarted)
                InvokeOnTimeEvent();
        }

        private void Start()
        {
            if (LapTimes == StartTime)
            {
                IsStarted = true;
                LapTimes = 0;
            }
        }

        private void InvokeOnTimeEvent() 
        {
            if (LapTimes == Interval)
            {
                Action.Invoke();
                LapTimes = 0;
            }
        }
    }
}
