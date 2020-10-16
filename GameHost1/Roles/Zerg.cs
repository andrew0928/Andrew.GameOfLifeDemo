namespace GameHost1.Roles
{
    public class Zerg
    {
        private bool _isAlive;
        public Zerg(bool isAlive)
        {
            this._isAlive = isAlive;
        }

        public bool IsAlive
        {
            get{ return _isAlive; }
        }

        public void Howl()
        {

        }

        public string Response()
        {
            return this._isAlive ? "hi" : string.Empty;
        }
    }
}