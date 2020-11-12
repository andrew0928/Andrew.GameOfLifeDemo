using System.Collections.Generic;

namespace GameHost1
{
    public interface ICell
    {
        int PosX { get; }
        int PosY { get; }
        bool IsAlive { get; }
        bool WillBeAlive { get; }
        void GetAlongWith(IEnumerable<ICell> cells);
    }
}
