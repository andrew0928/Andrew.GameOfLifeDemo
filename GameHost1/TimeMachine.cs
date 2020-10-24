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
            Now = Now.Add(duration);
        }
    }
}