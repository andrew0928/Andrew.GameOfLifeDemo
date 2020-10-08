namespace GameHost1
{
    public class Cell : ICell
    {
        private bool[,] _area;

        public bool IsLive { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area">must be bool[3, 3]</param>
        public Cell(bool[,] area)
        {
            _area = area;
            this.IsLive = _area[1, 1];
        }

        public bool Evolve()
        {
            var aroundLiveCellsCount = this.GetAroundLiveCellsCount();

            // rules of evolution
            if (this.IsLive)
            {
                if (aroundLiveCellsCount < 2 || aroundLiveCellsCount > 3)
                {
                    this.IsLive = false;
                }
            }
            else
            {
                if (aroundLiveCellsCount == 3)
                {
                    this.IsLive = true;
                }
            }

            return this.IsLive;
        }

        private ushort GetAroundLiveCellsCount()
        {
            ushort aroundLiveCellsCount = 0;

            for (int y = 0; y < _area.GetLength(0); y++)
            {
                for (int x = 0; x < _area.GetLength(1); x++)
                {
                    if (_area[x, y])
                    {
                        aroundLiveCellsCount++;
                    }
                }
            }

            if (this.IsLive)
            {
                aroundLiveCellsCount--;
            }

            return aroundLiveCellsCount;
        }
    }
}
