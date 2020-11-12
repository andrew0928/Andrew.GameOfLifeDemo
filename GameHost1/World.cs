using System.Threading;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameHost1
{
    public class World : IWorld
    {
        private int _width { get; set; }
        private int _depth { get; set; }
        private ILife[,] _matrix { get; set; }
        private int[,] _cellFrames { get; set; }
        private int _worldFrame { get; set; }
        private IDictionary<Guid, IList<int>> _positionDict = new Dictionary<Guid, IList<int>>();
        private HashSet<int> _cellFramesHashSet = new HashSet<int>();
        private Dictionary<int, TimeSpan> _cellFrameSwitchMoments = new Dictionary<int, TimeSpan>();

        public World(int width, int depth)
        {
            this._width = width;
            this._depth = depth;
        }

        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            if (init_matrix.GetLength(0) != _width || init_matrix.GetLength(1) != _depth) throw new ArgumentOutOfRangeException();
            _matrix = new ILife[_width, _depth];
            foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(_matrix))
            {
                var id = Guid.NewGuid();
                _matrix[x, y] = new Life(id, init_matrix[x, y], new GoogleMaps(this, id));
                _positionDict.Add(id, new List<int> { x, y });
            }

            _cellFrames = init_cell_frame;
            _worldFrame = world_frame;
            return true;
        }


        public ILife[,] GetNearbyData(Guid cellId)
        {
            if (_matrix == null) throw new ArgumentException();
            if (!_positionDict.ContainsKey(cellId)) throw new ArgumentException("Cell wasn't documented in dict.");
            var position = _positionDict[cellId];
            var r = position[0];
            var c = position[1];

            ILife[,] cells =
            {
                {_TryGetCell(r - 1, c - 1), _TryGetCell(r -1, c),_TryGetCell(r- 1, c + 1)},
                {_TryGetCell(r, c - 1), null,_TryGetCell(r, c + 1)},
                {_TryGetCell(r + 1, c - 1), _TryGetCell(r + 1, c),_TryGetCell(r + 1, c + 1)},
            };
            return cells;
        }

        private ILife _TryGetCell(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < _width && y < _depth) ? _matrix[x, y] : null;
        }

        private void _InitCellFramesHashSet()
        {
            foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(_matrix))
            {
                _cellFramesHashSet.Add(_cellFrames[x, y]);
            }
        }

        private void _UpdateCellFramesSwitchMoments(TimeSpan ts)
        {
            foreach (var h in _cellFramesHashSet)
            {
                if (ts.TotalMilliseconds % h == 0) _cellFrameSwitchMoments[h] = ts;
            }
        }

        private ILife[,] _GetUpdatedWorld(TimeSpan ts)
        {
            var updatedMatrix = new ILife[_width, _depth];
            foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(_matrix))
            {
                if (_cellFrameSwitchMoments.ContainsKey(_cellFrames[x, y]))
                {
                    var momentToBeSwitched = _cellFrameSwitchMoments[_cellFrames[x, y]];
                    var lastWorldFrame = ts - TimeSpan.FromMilliseconds(_worldFrame);
                    if (momentToBeSwitched > lastWorldFrame && momentToBeSwitched <= ts)
                    {
                        var updatedStatus = _matrix[x, y].GetUpdatedStatus();
                        var cloned = _matrix[x, y].Clone();
                        cloned.IsAlive = updatedStatus;
                        updatedMatrix[x, y] = cloned;
                        continue;
                    }
                }
                updatedMatrix[x, y] = _matrix[x, y];
            }
            return updatedMatrix;
        }

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until, bool realtime = false)
        {
            Stopwatch realtime_timer = new Stopwatch();
            realtime_timer.Start();
            var totalMSec = 0;

            _InitCellFramesHashSet();

            for (TimeSpan i = TimeSpan.FromMilliseconds(0); i <= until; i += TimeSpan.FromMilliseconds(1))
            {

                // 回傳 time == 0 的初始狀態
                if (i == TimeSpan.FromMilliseconds(0))
                {
                    yield return (i, _matrix);
                    continue;
                }

                _UpdateCellFramesSwitchMoments(i);

                if (i.TotalMilliseconds % _worldFrame == 0)
                {
                    var updatedMatrix = _GetUpdatedWorld(i);
                    _matrix = updatedMatrix;
                    var elapsed = realtime_timer.ElapsedMilliseconds;
                    if (realtime == true && elapsed < totalMSec)
                    {
                        Thread.Sleep((int)(totalMSec - elapsed));
                    }
                    yield return (i, updatedMatrix);
                }
                totalMSec++;
            }

        }
    }
}
