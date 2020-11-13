#define ENABLE_RUNNING_RECORDING
using System.Collections.Generic;
using System.Linq;

namespace GameHost1
{
    public class Cell : ICell
    {
        public int PosX { get; private set; }
        public int PosY { get; private set; }
        public int Generation { get; private set; }

        private int FrameDuration;
        private (bool isAlive, int startFrame) LifeState;
        private (bool isAlive, int startFrame) NextLifeState;

        public Cell(int posX, int posY, bool isAlive, int frameDuration, int startFrame)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.LifeState = (isAlive, startFrame);
            this.FrameDuration = frameDuration;
        }

        public bool IsAlive => this.LifeState.isAlive;

        public bool IsMyTurn(int currentFrame) => this.LifeState.startFrame <= currentFrame;
        public bool IsNextTurn(int nextFrame) => this.NextLifeState.startFrame <= nextFrame;

        public void GetAlongWith(IEnumerable<ICell> neighbors)
        {
            //var ncs = cells.Select(cell => $"({cell.PosX}, {cell.PosY}, {cell.IsAlive})");
            var value = neighbors.Select(x => x.IsAlive ? 1 : 0).Sum();

            //var beforeIsAlive = this.IsAlive;

            var nextIsAlive = false;
            if (this.IsAlive)
            {
                nextIsAlive = (value < 2 || value > 3) ? false : true;
            }
            else
            {
                nextIsAlive = (value == 3) ? true : false;
            }

            this.NextLifeState = (nextIsAlive, this.LifeState.startFrame + this.FrameDuration);

            //Console.WriteLine($"CurrentCell: ({this.PosX}, {this.PosY}, {beforeIsAlive} => {this.IsAlive}), Neighbors: {value}, {string.Join(", ", ncs)}");
        }

        public void NextGeneration()
        {
            this.LifeState = this.NextLifeState;
            this.Generation++;
        }
    }
}
