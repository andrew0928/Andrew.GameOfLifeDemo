using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class World
    {
        public int RowNum { get; set; }
        public int ColumnNum { get; set; }
        public ICell[,] Cells { get; set; }
        public IDictionary<Guid, IList<int>> PositionDict = new Dictionary<Guid, IList<int>>();
        public World(int rows, int columns)
        {
            Cells = new ICell[rows, columns];
            RowNum = rows;
            ColumnNum = columns;
        }
        public World GetNextGen()
        {
            ICell[,] nextGenCells = new ICell[RowNum, ColumnNum];
            // snapshot 到 nextGenCells 最後一起變
            for (int y = 0; y < ColumnNum; y++)
            {
                for (int x = 0; x < RowNum; x++)
                {
                    var updatedStatus = Cells[x, y].GetUpdatedStatus();
                    nextGenCells[x, y] = new Cell()
                    {
                        IsAlive = updatedStatus,
                        Id = Cells[x, y].Id,
                        LivesNumToLiveWhenAlive = Cells[x, y].LivesNumToLiveWhenAlive,
                        LivesNumToLiveWhenDead = Cells[x, y].LivesNumToLiveWhenDead,
                        GrowthRate = Cells[x, y].GrowthRate,
                        GoogleMaps = Cells[x,y].GoogleMaps
                    };
                }
            }
            Cells = nextGenCells;
            return this;
        }
        public ICell[,] GetNearbyData(Guid cellId)
        {
            if (Cells == null) throw new ArgumentException();
            if (!PositionDict.ContainsKey(cellId)) throw new ArgumentException("Cell wasn't documented in dict.");
            var position = PositionDict[cellId];
            var r = position[0];
            var c = position[1];

            ICell[,] cells =
            {
                {TryGetCell(r - 1, c - 1), TryGetCell(r -1, c),TryGetCell(r- 1, c + 1)},
                {TryGetCell(r, c - 1), null,TryGetCell(r, c + 1)},
                {TryGetCell(r + 1, c - 1), TryGetCell(r + 1, c),TryGetCell(r + 1, c + 1)},
            };
            return cells;
        }
        private ICell TryGetCell(int r, int c)
        {
            return (r >= 0 && c >= 0 && r < RowNum && c < ColumnNum) ? Cells[r, c] : null;
        }
        public void AddCell(int x, int y, bool isAlive)
        {
            if (x < 0 || x > RowNum || y < 0 || y > ColumnNum) throw new ArgumentOutOfRangeException();
            var cellId = Guid.NewGuid();
            Cells[x, y] = new Cell()
            {
                IsAlive = isAlive,
                Id = cellId,
                GoogleMaps = new GoogleMaps(this),
            };
            PositionDict.Add(cellId, new List<int> { x, y });
        }
        public void InitRandomMap(int rate)
        {
            Random rnd = new Random();
            for (int y = 0; y < ColumnNum; y++)
            {
                for (int x = 0; x < RowNum; x++)
                {
                    var cellId = Guid.NewGuid();
                    Cells[x, y] = new Cell()
                    {
                        IsAlive = rnd.Next(100) < rate,
                        Id = cellId,
                        GoogleMaps = new GoogleMaps(this),
                    };
                    PositionDict.Add(cellId, new List<int> { x, y });
                }
            }
        }
    }
}