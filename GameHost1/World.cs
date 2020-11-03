using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GameHost1
{
    public class World : IWorld
    {
        public readonly (int width, int depth) Dimation;
        private Life.Sensibility[,] _maps_current_life_sense;

        private int _frame;

        // 上一個 frame 的地圖快照。所有 life / god 的視覺都會觀察到這個 frame 的景物
        private Life[,] _maps_snapshot;

        private Dictionary<Life, (int x, int y)> _links = new Dictionary<Life, (int x, int y)>();


        public World(int width, int depth)
        {
            this.Dimation = (width, depth);
            this._maps_current_life_sense = new Life.Sensibility[this.Dimation.width, this.Dimation.depth];
            this._maps_snapshot = new Life[this.Dimation.width, this.Dimation.depth];
        }


        public World(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            this.Dimation = (init_matrix.GetLength(0), init_matrix.GetLength(1));
            this._maps_current_life_sense = new Life.Sensibility[this.Dimation.width, this.Dimation.depth];
            this._maps_snapshot = new Life[this.Dimation.width, this.Dimation.depth];

            if (this.Init(init_matrix, init_cell_frame, init_cell_start_frame, world_frame) == false) throw new ArgumentException();

        }


        private bool _is_init = false;
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            if (this._is_init) return false;
            if (this.Dimation.width != init_matrix.GetLength(0) || this.Dimation.depth != init_matrix.GetLength(1)) return false;
            if (this.Dimation.width != init_cell_frame.GetLength(0) || this.Dimation.depth != init_cell_frame.GetLength(1)) return false;
            if (this.Dimation.width != init_cell_start_frame.GetLength(0) || this.Dimation.depth != init_cell_start_frame.GetLength(1)) return false;

            this._frame = world_frame;

            foreach (var (x, y) in ForEachPos<bool>(init_matrix))
            {
                if (this._maps_current_life_sense[x, y] != null)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var cell = new Life(out var sense, init_matrix[x, y], init_cell_frame[x, y]);
                sense.InitWorldSide(this, (x, y), () =>
                {
                    return this.SeeAround((x, y));
                });
                this._maps_current_life_sense[x, y] = sense;

            }

            this._is_init = true;
            return true;
        }


        private int TimePass()
        {
            this.RefreshFrame();
            return this._frame;
        }

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until)
        {
            this.RefreshFrame();

            int until_frames = (int)until.TotalMilliseconds;
            SortedSet<ToDoItem> todoset = new SortedSet<ToDoItem>(new ToDoItemComparer());

            foreach(var (x, y) in ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            {
                var sense = this._maps_current_life_sense[x, y];
                todoset.Add(new ToDoItem()
                {
                    ID = sense.Itself.ID,
                    IsWorld = false,
                    TimePass = sense.TimePass,
                    NextTimeFrame = sense.TimePass()
                });
            }

            todoset.Add(new ToDoItem()
            {
                ID = -1,
                IsWorld = true,
                TimePass = this.TimePass,
                NextTimeFrame = this.TimePass()
            });

            do
            {
                var item = todoset.First();

                todoset.Remove(item);
                todoset.Add(new ToDoItem()
                {
                    ID = item.ID,
                    IsWorld = item.IsWorld,
                    TimePass = item.TimePass,
                    NextTimeFrame = item.NextTimeFrame + item.TimePass()
                });

                if (item.IsWorld) yield return (TimeSpan.FromMilliseconds(item.NextTimeFrame), this._maps_snapshot);
                if (item.NextTimeFrame >= until_frames) break;
            } while (true);
        }

        private class ToDoItem
        {
            public int ID;
            public bool IsWorld;
            public Func<int> TimePass;
            public int NextTimeFrame;
        }

        private class ToDoItemComparer : IComparer<ToDoItem>
        {
            public int Compare([AllowNull] ToDoItem x, [AllowNull] ToDoItem y)
            {
                if (x.NextTimeFrame == y.NextTimeFrame) return x.ID - y.ID;
                return x.NextTimeFrame - y.NextTimeFrame;
            }
        }

        private void RefreshFrame()
        {
            foreach (var (x, y) in ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            {
                this._maps_snapshot[x, y] = this._maps_current_life_sense[x, y].TakeSnapshot();
            }
        }


        // only life itself can do this
        private Life[,] SeeAround((int x, int y) pos)
        {
            Life[,] result = new Life[3, 3];

            result[0, 0] = this.SeePosition(pos.x - 1, pos.y - 1);
            result[1, 0] = this.SeePosition(pos.x   ,  pos.y - 1);
            result[2, 0] = this.SeePosition(pos.x + 1, pos.y - 1);

            result[0, 1] = this.SeePosition(pos.x - 1, pos.y   );
            //result[1, 1] = this.SeePosition(pos.x    , pos.y   );
            result[2, 1] = this.SeePosition(pos.x + 1, pos.y   );

            result[0, 2] = this.SeePosition(pos.x - 1, pos.y + 1);
            result[1, 2] = this.SeePosition(pos.x    , pos.y + 1);
            result[2, 2] = this.SeePosition(pos.x + 1, pos.y + 1);

            return result;
        }

        private Life SeePosition(int x, int y)
        {
            if (x < 0) return null;
            if (x >= this.Dimation.width) return null;
            if (y < 0) return null;
            if (y >= this.Dimation.depth) return null;
            return this._maps_snapshot[x, y];
        }

        // utility, 簡化到處都出現的雙層迴圈。只會循序取出 2D 陣列中所有的 (x, y) 座標組合
        public static IEnumerable<(int x, int y)> ForEachPos<T>(T[,] array)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                for (int x = 0; x < array.GetLength(0); x++)
                {
                    yield return (x, y);
                }
            }
        }
    }
}
