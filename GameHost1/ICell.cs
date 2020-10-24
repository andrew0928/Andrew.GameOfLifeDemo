using System.Collections.Generic;

namespace GameHost1
{
    public interface ICell
    {
        public bool IsAlive { get; set; }
        public IList<int> LivesNumToLiveWhenAlive { get; set; }
        public IList<int> LivesNumToLiveWhenDead { get; set; }
        public int GrowthRate { get; set; }
        public bool GetUpdatedStatus(int livesAround);
    }
}