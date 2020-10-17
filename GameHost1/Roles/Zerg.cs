using System;

namespace GameHost1.Roles
{
    public class Zerg
    {
        public bool IsAlive { get; private set; }

        public Zerg(bool isAlive)
        {
            this.IsAlive = isAlive;
        }

        public event EventHandler SendSingal;

        //收到訊號時
        public virtual void OnSignalReceived(object sender, EventArgs e)
        {
            if(sender is Zerg)
            {
                ((Zerg)sender).GetSingal(this.IsAlive ? "hi" : string.Empty);
            }
        }

        public void GetSingal(string signal)
        {
            if(string.IsNullOrWhiteSpace(signal) == false) _liveAroundMe++;
        }

        private int _liveAroundMe = 0;
        //演化到下一個世代
        public void Evolution(object sender, EventArgs e)
        {
            if (this.IsAlive)
            {
                if (_liveAroundMe < 2) { this.IsAlive = false; return; }
                if (_liveAroundMe == 2 || _liveAroundMe == 3) { this.IsAlive = true; return; }
                if (_liveAroundMe > 3) { this.IsAlive = false; return; }
            }

            this.IsAlive = _liveAroundMe == 3 ? true : false;
        }

        //過生活
        public void Live(object sender, EventArgs e) 
        {
            _liveAroundMe = 0;
            //這隻生物生存的目的就是吼叫
            SendSingal?.Invoke(this, new EventArgs());
        }
    }
}