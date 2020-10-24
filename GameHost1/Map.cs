using System;

namespace GameHost1
{
    public class Map
    {
        public int RowNum { get; set; }
        public int ColumnNum { get; set; }
        public ICell[,] Cells { get; set; }
        public double SecondsToSwitch => 1 / 60;
        public Map(ICell[,] cells)
        {
            Cells = cells;
            RowNum = cells.GetLength(0);
            ColumnNum = cells.GetLength(1);
        }
        public Map(int rows, int columns)
        {
            Cells = new ICell[rows, columns];
            RowNum = rows;
            ColumnNum = columns;
            initRandomMap(20);
        }
        public Map GetNextGen()
        {
            ICell[,] nextGenCells = new ICell[RowNum, ColumnNum];
            // snapshot 到 nextGenCells 最後一起變
            for (int y = 0; y < ColumnNum; y++)
            {
                for (int x = 0; x < RowNum; x++)
                {
                    // 看附近八格的細胞有誰活著
                    var livesAround = 0;
                    for (int ay = y - 1; ay <= y + 1; ay++)
                    {
                        for (int ax = x - 1; ax <= x + 1; ax++)
                        {
                            // 超過邊界的不算
                            if (ax < 0 || ay < 0 || ax >= RowNum || ay >= ColumnNum) continue;
                            // 自己不算
                            if (ax == x && ay == y) continue;
                            if (Cells[ax, ay].IsAlive == true) livesAround++;
                        }
                    }
                    var updatedStatus = Cells[x, y].GetUpdatedStatus(livesAround);
                    nextGenCells[x, y] = new Cell()
                    {
                        IsAlive = updatedStatus,
                        LivesNumToLiveWhenAlive = Cells[x, y].LivesNumToLiveWhenAlive,
                        LivesNumToLiveWhenDead = Cells[x, y].LivesNumToLiveWhenDead,
                        GrowthRate = Cells[x, y].GrowthRate,
                    };
                }
            }
            Cells = nextGenCells;
            return this;
        }
        private void initRandomMap(int rate)
        {
            Random rnd = new Random();
            for (int y = 0; y < ColumnNum; y++)
            {
                for (int x = 0; x < RowNum; x++)
                {
                    Cells[x, y] = new Cell()
                    {
                        IsAlive = rnd.Next(100) < rate
                    };
                }
            }
        }
    }
}