using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class Life : ILife
    {
        private Guid _id { get; set; }
        public bool IsAlive { get; set; }
        private IList<int> _livesNumToLiveWhenAlive { get; set; } = new List<int>() { 2, 3 };
        private IList<int> _livesNumToLiveWhenDead { get; set; } = new List<int>() { 3 };
        private GoogleMaps _googleMaps { get; set; }
        public Life(Guid id, bool isAlive, GoogleMaps googleMaps)
        {
            _id = id;
            IsAlive = isAlive;
            _googleMaps = googleMaps;
        }
        public bool GetUpdatedStatus()
        {
            var cellsAround = _googleMaps.ShowNearbyView();
            var livesAround = 0;
            foreach (var item in cellsAround)
            {
                if (item == null) continue;
                if (item.IsAlive == true) livesAround++;
            }
            if (IsAlive == true && _livesNumToLiveWhenAlive.Contains(livesAround)) return true;
            if (IsAlive == false && _livesNumToLiveWhenDead.Contains(livesAround)) return true;
            return false;
        }

        public ILife Clone()
        {
            return new Life(_id, IsAlive, _googleMaps);
        }
    }
}
