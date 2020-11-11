using System.Threading;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameHost1
{
    public class World : IWorld
    {
        public int Width { get; set; }
        public int Depth { get; set; }
        public ILife[,] Matrix { get; set; }
        public int[,] CellFrames { get; set; }
        public int WorldFrame { get; set; }
        public IDictionary<Guid, IList<int>> PositionDict = new Dictionary<Guid, IList<int>>();

        public World(int width, int depth)
        {
            this.Width = width;
            this.Depth = depth;
        }

        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            if (init_matrix.GetLength(0) != Width || init_matrix.GetLength(1) != Depth) throw new ArgumentOutOfRangeException();
            Matrix = new ILife[Width, Depth];
            foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(Matrix))
            {
                var id = Guid.NewGuid();
                Matrix[x, y] = new Life(id, init_matrix[x, y], new GoogleMaps(this, id));
                PositionDict.Add(id, new List<int> { x, y });
            }

            CellFrames = init_cell_frame;
            WorldFrame = world_frame;
            return true;
        }


        public ILife[,] GetNearbyData(Guid cellId)
        {
            if (Matrix == null) throw new ArgumentException();
            if (!PositionDict.ContainsKey(cellId)) throw new ArgumentException("Cell wasn't documented in dict.");
            var position = PositionDict[cellId];
            var r = position[0];
            var c = position[1];

            ILife[,] cells =
            {
                {TryGetCell(r - 1, c - 1), TryGetCell(r -1, c),TryGetCell(r- 1, c + 1)},
                {TryGetCell(r, c - 1), null,TryGetCell(r, c + 1)},
                {TryGetCell(r + 1, c - 1), TryGetCell(r + 1, c),TryGetCell(r + 1, c + 1)},
            };
            return cells;
        }

        private ILife TryGetCell(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < Width && y < Depth) ? Matrix[x, y] : null;
        }

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until, bool realtime = false)
        {
            Stopwatch realtime_timer = new Stopwatch();
            realtime_timer.Restart();

            var cellFrameSwitchMoments = new Dictionary<int, TimeSpan>();

            for (TimeSpan i = TimeSpan.FromMilliseconds(0); i <= until; i += TimeSpan.FromMilliseconds(1))
            {

                // 回傳 time == 0 的初始狀態
                if (i == TimeSpan.FromMilliseconds(0))
                {
                    yield return (i, Matrix);
                    continue;
                }

                // 新增切換細胞生死狀態的一筆紀錄
                foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(Matrix))
                {
                    // TODO: 可以怎麼簡化成每種 frame 的細胞加一次即可?
                    if (i.TotalMilliseconds % CellFrames[x, y] == 0)
                    {
                        cellFrameSwitchMoments[CellFrames[x, y]] = i; // 10, 20, 30..
                    }
                }

                // 切換世界：按照切換細胞紀錄切換細胞
                if (i.TotalMilliseconds % WorldFrame == 0)
                {
                    var updatedMatrix = new ILife[Width, Depth];
                    foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(Matrix))
                    {
                        updatedMatrix[x, y] = Matrix[x, y];

                        var momentToBeSwitched = cellFrameSwitchMoments[CellFrames[x, y]];
                        var lastWorldFrame = i - TimeSpan.FromMilliseconds(WorldFrame);
                        if (momentToBeSwitched > lastWorldFrame && momentToBeSwitched <= i)
                        {
                            var updatedStatus = Matrix[x, y].GetUpdatedStatus();
                            updatedMatrix[x, y] = new Life(Matrix[x, y].Id, updatedStatus, Matrix[x, y].GoogleMaps);
                        }
                    }
                    Matrix = updatedMatrix;
                    yield return (i, updatedMatrix);
                }
            }

            if (realtime == true && realtime_timer.ElapsedMilliseconds < 10)
            {
                Thread.Sleep((int)(10 - realtime_timer.ElapsedMilliseconds));
            }
        }
    }
}
