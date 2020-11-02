using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class GoogleMaps
    {
        public World World { get; set; }
        public GoogleMaps(World world)
        {
            World = world;
        }
        public ICell[,] ShowNearbyView(Guid cellId)
        {
            return World.GetNearbyData(cellId);
        }
    }
}