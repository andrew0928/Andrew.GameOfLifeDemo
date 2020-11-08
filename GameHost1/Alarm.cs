using System;
using System.Timers;

namespace GameHost1
{
    public class Alarm
    {
        private Timer Timer { get; set; }

        private Action Action { get; set; }

        private int Interval { get; set; }

        public Alarm(int interval, Action action) 
        {
            this.Action = action;
            this.Interval = interval;
        }

        public void Start(int proportion) 
        {
            this.Timer = new Timer(this.Interval / proportion);
            this.Timer.Elapsed += OnTimedEvent;
            this.Timer.AutoReset = true;
            this.Timer.Enabled = true;

            this.Timer.Start();
        }

        public void Stop() 
        {
            this.Timer.Stop();
        }

        public void Dispose() 
        {
            this.Timer.Dispose();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Action.Invoke();
        }
    }
}
