using System;
using System.Collections.Generic;

namespace GameHost1
{
    public interface ILife
    {
        public Guid Id { get; set; }
        public bool IsAlive { get; set; }
        public IList<int> LivesNumToLiveWhenAlive { get; set; }
        public IList<int> LivesNumToLiveWhenDead { get; set; }
        public GoogleMaps GoogleMaps { get; set; }
        public bool GetUpdatedStatus();
    }
}
