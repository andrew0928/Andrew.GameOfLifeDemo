#define ENABLE_RUNNING_RECORDING
using System.Collections.Generic;
using System.Linq;

namespace GameHost1
{
    public class Cell : ICell
    {
        public int PosX { get; private set; }
        public int PosY { get; private set; }
        public bool IsAlive { get; private set; }
        public bool WillBeAlive { get; private set; }
        public int Generation { get; private set; }

        private int FrameDuration;
        private int StartFrame;

        public Cell(int posX, int posY, bool isAlive, int frameDuration, int startFrame)
        {
            this.PosX = posX;
            this.PosY = posY;
            this.IsAlive = isAlive;
            this.FrameDuration = frameDuration;
            this.StartFrame = startFrame;
        }

        public void GetAlongWith(IEnumerable<ICell> cells)
        {
            //var ncs = cells.Select(cell => $"({cell.PosX}, {cell.PosY}, {cell.IsAlive})");
            var value = cells.Select(x => x.IsAlive ? 1 : 0).Sum();

            var beforeIsAlive = this.IsAlive;

            if (this.IsAlive)
            {
                this.WillBeAlive = (value < 2 || value > 3) ? false : true;
            }
            else
            {
                this.WillBeAlive = (value == 3) ? true : false;
            }

            //Console.WriteLine($"CurrentCell: ({this.PosX}, {this.PosY}, {beforeIsAlive} => {this.IsAlive}), Neighbors: {value}, {string.Join(", ", ncs)}");
        }

        public void NextGeneration()
        {
            this.IsAlive = this.WillBeAlive;
            this.Generation++;
        }
    }
}
