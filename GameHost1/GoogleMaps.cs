using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class GoogleMaps
    {
        private World _world { get; set; }
        private Guid _cellId { get; set; }
        public GoogleMaps(World world, Guid cellId)
        {
            _world = world;
            _cellId = cellId;
        }
        public ILife[,] ShowNearbyView()
        {
            return _world.GetNearbyData(_cellId);
        }
    }
}