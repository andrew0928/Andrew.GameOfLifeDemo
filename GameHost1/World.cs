using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        public ILife[,] SeeAround(int x, int y);
    }

    /// <summary>
    /// 驅動這整個世界 realtime 演進的所有生物必須實作的 interface.
    /// 公開的介面代表: 每次演化後透過 yield return 告知環境，下次演化預計的時間點 ( int, 單位 msec )。
    /// 
    /// 透過 C# 編譯器支援的 yield return 來實作世界的排程器與每個生命之間的協作。
    /// </summary>
    public interface IRunningObject
    {
        public int ID { get; }
        public IEnumerable<int> AsTimePass();
        public int Age { get; }
    }


    public class World : IWorld, IRunningObject, ILifeVision
    {
        public readonly (int width, int depth) Dimation;
        private Life.Sensibility[,] _maps_current_life_sense;

        private int _frame;
        private int _time_passed = 0;

        // 上一個 frame 的地圖快照。所有 life / god 的視覺都會觀察到這個 frame 的景物
        private ILife[,] _maps_snapshot;


        public World(int width, int depth)
        {
            this.Dimation = (width, depth);
            this._maps_current_life_sense = new Life.Sensibility[this.Dimation.width, this.Dimation.depth];
            this._maps_snapshot = new ILife[this.Dimation.width, this.Dimation.depth];
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
                sense.InitWorldSide(this, (x, y));
                this._maps_current_life_sense[x, y] = sense;

            }

            this._is_init = true;
            return true;
        }

        public override string ToString()
        {
            return $"world(id: {((IRunningObject)this).ID}, age: {((IRunningObject)this).Age})";
        }

        int IRunningObject.Age
        {
            get
            {
                return this._time_passed;
            }
        }

        int IRunningObject.ID
        {
            get { return int.MaxValue; }
        }

        IEnumerable<int> IRunningObject.AsTimePass()
        {
            while(true)
            {
                this._time_passed += this._frame;
                yield return this._time_passed;
                Debug.WriteLine($"- world: {this._time_passed}");
            }
        }


        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until, bool realtime = false)
        {
            
            if (!this._is_init) throw new InvalidOperationException();

            this.RefreshFrame();
            int until_frames = (int)Math.Min(int.MaxValue, until.TotalMilliseconds);

            //yield return (TimeSpan.Zero, this._maps_snapshot);

            SortedSet<RunningObjectRecord> todoset = new SortedSet<RunningObjectRecord>();
            //PriorityQueue<RunningObjectRecord> pqlist = new PriorityQueue<RunningObjectRecord>();

            foreach (var (x, y) in ArrayHelper.ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            {
                var sense = this._maps_current_life_sense[x, y];
                todoset.Add(new RunningObjectRecord(sense.Itself));
                //pqlist.Enqueue(new RunningObjectRecord(sense.Itself));
            }
            todoset.Add(new RunningObjectRecord(this));
            //pqlist.Enqueue(new RunningObjectRecord(this));


            // world start
            Stopwatch timer = new Stopwatch();
            timer.Restart();
            int now = 0;
            do
            {
                var item = todoset.Min;
                todoset.Remove(item);
                //var item = pqlist.Dequeue();

                if (item.Enumerator.Current >= until_frames) break;
                now = item.Enumerator.Current;


                if (item.Enumerator.MoveNext())
                {
                    todoset.Add(item);
                    //pqlist.Enqueue(item);
                    if (realtime) SpinWait.SpinUntil(() => { return timer.ElapsedMilliseconds >= now; });
                }
                else
                {
                    // yield break. means the life was terminated.
                    continue;
                }

                if (item.Source is World && now > 0)    // 按照 unit test 的規則，第一 round 應該是第一次演化的結果。不包含初始化的狀態。
                {
                    Debug.WriteLine($"- running: {now}");
                    this.RefreshFrame();
                    yield return (TimeSpan.FromMilliseconds(now), this._maps_snapshot);
                }

            } while (true);
        }




        private void RefreshFrame()
        {
            foreach (var (x, y) in ArrayHelper.ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            {
                this._maps_snapshot[x, y] = new LifeSnapshot(this._maps_current_life_sense[x, y].Itself.IsAlive, this._maps_current_life_sense[x, y].Itself.Generation);
            }
        }

        ILife[,] ILifeVision.SeeAround(int x, int y)
        {
            var pos = (x, y);
            ILife[,] result = new ILife[3, 3];

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

        private ILife SeePosition(int x, int y)
        {
            if (x < 0) return null;
            if (y < 0) return null;
            if (x >= this.Dimation.width) return null;
            if (y >= this.Dimation.depth) return null;
            return this._maps_snapshot[x, y];
        }


        private class RunningObjectRecord : IComparable<RunningObjectRecord>
        {
            public IRunningObject Source { get; private set; }

            public IEnumerator<int> Enumerator { get; private set; }

            public RunningObjectRecord(IRunningObject source)
            {
                this.Source = source;
                this.Enumerator = this.Source.AsTimePass().GetEnumerator();
                //if (!this.Enumerator.MoveNext()) throw new InvalidOperationException();
            }
            public override string ToString()
            {
                return this.Source.ToString();
            }

            public int CompareTo(RunningObjectRecord other)
            {
                if (this.Source.Age == other.Source.Age)
                {
                    //if (this.Source is World) return 1;
                    //if (other.Source is World) return -1;
                    return this.Source.ID.CompareTo(other.Source.ID); // normal order
                }
                else
                {
                    return this.Source.Age.CompareTo(other.Source.Age);
                }

                //return (this.Source.Age * 10000 + this.Source.ID) - (other.Source.Age * 10000 + other.Source.ID);
            }
        }

    }
}
