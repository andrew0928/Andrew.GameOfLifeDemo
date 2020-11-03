using System;
using System.Timers;

namespace GameHost1
{
    public class Alarm
    {
        public Timer Timer { get; set; }
        public Action Action { get; set; }

        public Alarm(Action action) 
        {
            this.Action = action;

            Random rnd = new Random();
            var temp = rnd.Next(100) + 100;

            this.Timer = new Timer(10);
            this.Timer.Elapsed += OnTimedEvent;
            this.Timer.AutoReset = true;
            this.Timer.Enabled = true;
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
