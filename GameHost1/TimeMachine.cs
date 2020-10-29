using System;

namespace GameHost1
{
    public class TimeMachine
    {
        public DateTime Now { get; set; }
        public TimeMachine()
        {
            Now = DateTime.Now;
        }
        public void PassTime(TimeSpan duration)
        {
            // TODO: trigger map GetNextGen every 1/60 sec
            Now = Now.Add(duration);
        }
    }
}