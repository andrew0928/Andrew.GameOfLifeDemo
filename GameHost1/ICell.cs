using System;
using System.Collections.Generic;

namespace GameHost1
{
    public interface ICell
    {
        Guid Id { get; set; }
        bool IsAlive { get; set; }
        IList<int> LivesNumToLiveWhenAlive { get; set; }
        IList<int> LivesNumToLiveWhenDead { get; set; }
        GoogleMaps GoogleMaps { get; set; }
        int GrowthRate { get; set; }
        bool GetUpdatedStatus();
    }
}