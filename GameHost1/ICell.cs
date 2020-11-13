#define ENABLE_RUNNING_RECORDING
using System.Collections.Generic;

namespace GameHost1
{
    public interface ICell
    {
        int PosX { get; }
        int PosY { get; }
        bool IsAlive { get; }
        int Generation { get; }

        bool IsMyTurn(int currentFrame);
        bool IsNextTurn(int currentFrame);
        void NextGeneration();
        void GetAlongWith(IEnumerable<ICell> cells);
    }
}
