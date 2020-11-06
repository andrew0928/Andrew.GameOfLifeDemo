using System;
using System.Timers;

namespace GameHost1
{
    public class Alarm
    {
        public Timer Timer { get; set; }
        public Action Action { get; set; }

        public Alarm(int interval, Action action) 
        {
            this.Action = action;
            this.Timer = new Timer(interval);
            this.Timer.Elapsed += OnTimedEvent;
            this.Timer.AutoReset = true;
            this.Timer.Enabled = true;

            this.Stop();
        }
        public void Start() 
        {
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
