namespace GameHost1
{
    public class Life : ILife
    {
        public bool IsAlive { get; }
        public Life(bool isAlive)
        {
            IsAlive = isAlive;
        }
    }
}
