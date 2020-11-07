using System.Linq;
using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class World : IWorld
    {
        public int Width { get; set; }
        public int Depth { get; set; }
        public ILife[,] Matrix { get; set; }
        public int[,] CellFrame { get; set; }
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
            for (int y = 0; y < Depth; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var id = Guid.NewGuid();
                    Matrix[x, y] = new Life(id, init_matrix[x, y], new GoogleMaps(this, id));
                    PositionDict.Add(id, new List<int> { x, y });
                }
            }
            CellFrame = init_cell_frame;
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

        /// <summary>
        /// 根據經過的時間長度和定義過的 frame, 回傳每個時間點的 Matrix
        /// </summary>
        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until)
        {
            var lastWorldFrame = TimeSpan.FromMilliseconds(0);
            var cellFrameSwitchMoments = new Dictionary<int, List<TimeSpan>>();
            for (TimeSpan i = TimeSpan.FromMilliseconds(0); i <= until; i += TimeSpan.FromMilliseconds(1))
            {
                // 新增切換細胞紀錄
                for (int y = 0; y < Depth; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        var cellFrame = CellFrame[x, y];
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
                }

                // 切換世界：按照切換細胞紀錄切換細胞
                if (i - lastWorldFrame == TimeSpan.FromMilliseconds(WorldFrame))
                {
                    var updatedMatrix = new ILife[Width, Depth];
                    for (int y = 0; y < Depth; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            var moments = cellFrameSwitchMoments[CellFrame[x, y]];
                            var currentFrame = i;
                            // 從 lastFrame 到 currentFrame 如果細胞有動靜再更新
                            if (moments.Find(x => x > lastWorldFrame && x <= currentFrame) != null)
                            {
                                var updatedStatus = Matrix[x, y].GetUpdatedStatus();
                                updatedMatrix[x, y] = new Life(Matrix[x, y].Id, updatedStatus, Matrix[x, y].GoogleMaps);
                            }
                            else
                            {
                                updatedMatrix[x, y] = Matrix[x, y];
                            }
                        }
                    }
                    Matrix = updatedMatrix;
                    yield return (i, updatedMatrix);
                    lastWorldFrame = i;
                }
            }
        }
    }
}
