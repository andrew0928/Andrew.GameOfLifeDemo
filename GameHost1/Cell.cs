using System.Collections.Generic;

namespace GameHost1
{
    public class Cell : ICell
    {
        public bool IsAlive { get; set; }
        // TODO: 有 rules 可以統一 config?
        public IList<int> LivesNumToLiveWhenAlive { get; set; }
        public IList<int> LivesNumToLiveWhenDead { get; set; }
        public int GrowthRate { get; set; }
        public Cell()
        {
            LivesNumToLiveWhenAlive = new List<int>() { 2, 3 };
            LivesNumToLiveWhenDead = new List<int>() { 3 };
        }
        public bool GetUpdatedStatus(int livesAround)
        {
            if (IsAlive == true && LivesNumToLiveWhenAlive.Contains(livesAround)) return true;
            if (IsAlive == false && LivesNumToLiveWhenDead.Contains(livesAround)) return true;
            return false;
        }
    }
}