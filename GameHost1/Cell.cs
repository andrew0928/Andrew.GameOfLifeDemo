using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class Cell : ICell
    {
        public Guid Id { get; set; }
        public bool IsAlive { get; set; }
        public IList<int> LivesNumToLiveWhenAlive { get; set; } = new List<int>() { 2, 3 };
        public IList<int> LivesNumToLiveWhenDead { get; set; } = new List<int>() { 3 };
        public GoogleMaps GoogleMaps { get; set; }
        public int GrowthRate { get; set; }
        public bool GetUpdatedStatus()
        {
            var cellsAround = GoogleMaps.ShowNearbyView(Id);
            var livesAround = 0;
            foreach (var item in cellsAround)
            {
                if (item == null) continue;
                if (item.IsAlive == true) livesAround++;
            }
            if (IsAlive == true && LivesNumToLiveWhenAlive.Contains(livesAround)) return true;
            if (IsAlive == false && LivesNumToLiveWhenDead.Contains(livesAround)) return true;
            return false;
        }
    }
}