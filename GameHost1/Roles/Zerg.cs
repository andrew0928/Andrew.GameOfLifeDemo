using System;

namespace GameHost1.Roles
{
    public class Zerg
    {
        private bool _isAlive;
        private int _liveAroundMe = 0;      

        public Zerg(bool isAlive)
        {
            this._isAlive = isAlive;
        }

        public bool IsAlive()
        {
            this.SendSignal();
            return _isAlive;
        }

        public event EventHandler SendSingal;

        //收到訊號時
        public virtual void OnSignalReceived(object sender, EventArgs e)
        {
            if(sender is Zerg)
            {
                ((Zerg)sender).GetSingal(this._isAlive ? "hi" : string.Empty);
            } 
        }

        public void GetSingal(string signal)
        {
            if(string.IsNullOrWhiteSpace(signal) == false) _liveAroundMe++;
        }

        private void SendSignal()
        {
             _liveAroundMe = 0;

            SendSingal?.Invoke(this, new EventArgs());

            if(this._isAlive)
            {
                if(_liveAroundMe < 2){ this._isAlive = false; return;}
                if(_liveAroundMe ==2 || _liveAroundMe == 3){ this._isAlive = true; return;}
                if(_liveAroundMe > 3){ this._isAlive = false; return;}
            }

            this._isAlive = _liveAroundMe == 3 ? true : false;
        }
    }
}