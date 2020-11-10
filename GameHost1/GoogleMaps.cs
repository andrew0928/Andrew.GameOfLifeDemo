using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class GoogleMaps
    {
        public World World { get; set; }
        public Guid CellId { get; set; }
        public GoogleMaps(World world, Guid cellId)
        {
            World = world;
            CellId = cellId;
        }
        public ILife[,] ShowNearbyView()
        {
            return World.GetNearbyData(CellId);
        }
    }
}