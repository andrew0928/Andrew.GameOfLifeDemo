using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameHost1
{
    /// <summary>
    /// 提供 Life 視覺能力的 interface, 應該由 World 來提供 implementation.
    /// </summary>
    public interface ILifeVision
    {
        public Life[,] SeeAround(int x, int y);
    }

    /// <summary>
    /// 驅動這整個世界 realtime 演進的所有生物必須實作的 interface.
    /// 公開的介面代表: 每次演化後透過 yield return 告知環境，下次演化預計的時間點 ( int, 單位 msec )。
    /// 
    /// 透過 C# 編譯器支援的 yield return 來實作世界的排程器與每個生命之間的協作。
    /// </summary>
    public interface IRunningObject
    {
        public IEnumerable<int> AsTimePass();
    }


    public class World : IWorld, IRunningObject, ILifeVision
    {
        public readonly (int width, int depth) Dimation;
        private Life.Sensibility[,] _maps_current_life_sense;

        private int _frame;
        private int _time_passed = 0;

        // 上一個 frame 的地圖快照。所有 life / god 的視覺都會觀察到這個 frame 的景物
        private Life[,] _maps_snapshot;

        private Dictionary<Life, (int x, int y)> _links = new Dictionary<Life, (int x, int y)>();


        public World(int width, int depth)
        {
            this.Dimation = (width, depth);
            this._maps_current_life_sense = new Life.Sensibility[this.Dimation.width, this.Dimation.depth];
            this._maps_snapshot = new Life[this.Dimation.width, this.Dimation.depth];
        }


        private bool _is_init = false;
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            if (this._is_init) return false;
            if (this.Dimation.width != init_matrix.GetLength(0) || this.Dimation.depth != init_matrix.GetLength(1)) return false;
            if (this.Dimation.width != init_cell_frame.GetLength(0) || this.Dimation.depth != init_cell_frame.GetLength(1)) return false;
            if (this.Dimation.width != init_cell_start_frame.GetLength(0) || this.Dimation.depth != init_cell_start_frame.GetLength(1)) return false;

            this._frame = world_frame;

            foreach (var (x, y) in ArrayHelper.ForEachPos<bool>(init_matrix))
            {
                if (this._maps_current_life_sense[x, y] != null)
                {
                    throw new ArgumentOutOfRangeException();
                }

                var cell = new Life(out var sense, init_matrix[x, y], init_cell_frame[x, y]);
                //sense.InitWorldSide(this, (x, y), () =>
                //{
                //    return this.SeeAround((x, y));
                //});
                sense.InitWorldSide(this, (x, y));
                this._maps_current_life_sense[x, y] = sense;

            }

            this._is_init = true;
            return true;
        }


        IEnumerable<int> IRunningObject.AsTimePass()
        {
            while(true)
            {
                this.RefreshFrame();
                yield return this._time_passed += this._frame;
            }
        }


        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until, bool realtime = false)
        {
            if (!this._is_init) throw new InvalidOperationException();

            this.RefreshFrame();
            int until_frames = (int)Math.Min(int.MaxValue, until.TotalMilliseconds);

            SortedSet<RunningObjectRecord> todoset = new SortedSet<RunningObjectRecord>(new RunningObjectRecord.Comparer());

            foreach(var (x, y) in ArrayHelper.ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            {
                var sense = this._maps_current_life_sense[x, y];
                todoset.Add(new RunningObjectRecord(sense.Itself));
            }
            todoset.Add(new RunningObjectRecord(this));


            // world start
            Stopwatch timer = new Stopwatch();
            timer.Restart();
            do
            {
                var item = todoset.First();

                todoset.Remove(item);

                if (item.Enumerator.MoveNext())
                {
                    todoset.Add(item);
                    if (realtime) SpinWait.SpinUntil(() => { return timer.ElapsedMilliseconds >= item.Enumerator.Current; });
                }
                else
                {
                    // yield break. means the life was terminated.
                    continue;
                }

                if (item.Source is World) yield return (TimeSpan.FromMilliseconds(item.Enumerator.Current), this._maps_snapshot);
                if (item.Enumerator.Current >= until_frames) break;

            } while (true);
        }




        private void RefreshFrame()
        {
            foreach (var (x, y) in ArrayHelper.ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            {
                this._maps_snapshot[x, y] = this._maps_current_life_sense[x, y].TakeSnapshot();
            }
        }

        Life[,] ILifeVision.SeeAround(int x, int y)
        {
            var pos = (x, y);
        //    throw new NotImplementedException();
        //}
        //// only life itself can do this
        //private Life[,] SeeAround((int x, int y) pos)
        //{
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







        private class RunningObjectRecord
        {
            public IRunningObject Source { get; private set; }

            public IEnumerator<int> Enumerator { get; private set; }

            public RunningObjectRecord(IRunningObject source)
            {
                this.Source = source;
                this.Enumerator = this.Source.AsTimePass().GetEnumerator();
                if (!this.Enumerator.MoveNext()) throw new InvalidOperationException();
            }

            public class Comparer : IComparer<RunningObjectRecord>
            {
                public int Compare([AllowNull] RunningObjectRecord x, [AllowNull] RunningObjectRecord y)
                {
                    if (x.Enumerator.Current == y.Enumerator.Current)
                    {
                        return
                            ((x.Source is World) ? (-1) : ((x.Source as Life).ID)) -
                            ((y.Source is World) ? (-1) : ((y.Source as Life).ID));
                    }
                    return x.Enumerator.Current - y.Enumerator.Current;
                }
            }
        }

    }
}
