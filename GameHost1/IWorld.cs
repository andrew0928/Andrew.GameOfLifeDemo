using System.Collections.Generic;

namespace GameHost1
{
    public interface IWorld
    {
        IEnumerable<ICell> GetNeighbors(ICell cell, int redius = 1);
        IEnumerable<ICell> Traverse();
        bool[,] NextMoment();
    }
}
