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
        private IDictionary<Guid, IList<int>> positionDict = new Dictionary<Guid, IList<int>>();
        private IDictionary<int, List<TimeSpan>> cellFrameSwitchMoments = new Dictionary<int, List<TimeSpan>>();

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
                positionDict.Add(id, new List<int> { x, y });
            }

            CellFrames = init_cell_frame;
            WorldFrame = world_frame;
            return true;
        }


        public ILife[,] GetNearbyData(Guid cellId)
        {
            if (Matrix == null) throw new ArgumentException();
            if (!positionDict.ContainsKey(cellId)) throw new ArgumentException("Cell wasn't documented in dict.");
            var position = positionDict[cellId];
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
            var lastWorldFrame = TimeSpan.FromMilliseconds(0);

            for (TimeSpan i = TimeSpan.FromMilliseconds(0); i <= until; i += TimeSpan.FromMilliseconds(10))
            {
                Stopwatch realtime_timer = new Stopwatch();
                realtime_timer.Restart();

                // 新增切換細胞生死狀態的一筆紀錄 (但還沒切換)
                foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(Matrix))
                {
                    var cellFrame = CellFrames[x, y];
                    var lastCellFrame = cellFrameSwitchMoments.ContainsKey(cellFrame) ? cellFrameSwitchMoments[cellFrame].Max() : TimeSpan.FromMilliseconds(0);
                    if (i - lastCellFrame == TimeSpan.FromMilliseconds(cellFrame))
                    {
                        if (!cellFrameSwitchMoments.ContainsKey(cellFrame))
                        {
                            cellFrameSwitchMoments[cellFrame] = new List<TimeSpan>();
                        }
                        cellFrameSwitchMoments[cellFrame].Add(i); // 10, 20, 30..
                    }
                }

                // 回傳 time == 0 的初始狀態
                if (i == TimeSpan.FromMilliseconds(0)) yield return (i, Matrix);

                // 切換世界：按照切換細胞紀錄切換細胞
                if (i - lastWorldFrame == TimeSpan.FromMilliseconds(WorldFrame))
                {
                    var updatedMatrix = new ILife[Width, Depth];
                    foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(Matrix))
                    {
                        var cellFrame = CellFrames[x, y];
                        if (!cellFrameSwitchMoments.ContainsKey(cellFrame))
                        {
                            // 還不需要切換生死狀態
                            updatedMatrix[x, y] = Matrix[x, y];
                            continue;
                        }


                        List<TimeSpan> moments = cellFrameSwitchMoments[cellFrame];
                        var currentFrame = i;
                        // 從 lastFrame 到 currentFrame 如果細胞有動靜再更新
                        if (moments.Find(x => x > lastWorldFrame && x <= currentFrame) != null)
                        {
                            var updatedStatus = Matrix[x, y].GetUpdatedStatus();
                            updatedMatrix[x, y] = new Life(Matrix[x, y].Id, updatedStatus, Matrix[x, y].GoogleMaps);
                        }
                        else
                        {
                            // 還不需要切換生死狀態
                            updatedMatrix[x, y] = Matrix[x, y];
                        }
                    }
                    Matrix = updatedMatrix;
                    yield return (i, updatedMatrix);
                    lastWorldFrame = i;
                }

                if (realtime == true && realtime_timer.ElapsedMilliseconds < 10)
                {
                    Thread.Sleep((int)(10 - realtime_timer.ElapsedMilliseconds));
                }
            }
        }
    }
}
