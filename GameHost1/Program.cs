using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameHost1
{
    class Program
    {
        static void Main(string[] args)
        {
            // configure
            int total_cycle_count = 5000;
            int map_fill_rate = 80;
            (int x, int y) map_size = (100, 50);
            
            
            // init
            bool[,] matrix = new bool[map_size.x, map_size.y];
            Init(matrix, map_fill_rate);
            Console.Clear();


            // run
            for (int count = 0; count < total_cycle_count; count++)
            {
                int live_count = 0;
                Thread.Sleep(200);

                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        var cell = new Life(matrix[x, y], (x, y));
                        var mapcx = new MapContext(matrix, cell);
                        matrix[x, y] = cell.NextCycle(mapcx);

                        Console.Write(matrix[x, y]? '★' : '☆');
                        if (matrix[x, y]) live_count++;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"world size: ({map_size.x}, {map_size.y}), life fill rate: {map_fill_rate} %");
                Console.WriteLine($"total lives: {live_count}, round: {count} / {total_cycle_count}...");
            }
        }


        static void Init(bool[,] matrix, int rate)
        {
            Random rnd = new Random();

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    matrix[x, y] = (rnd.Next(100) < rate);
                }
            }
        }
    }


    public class Life
    {
        public bool IsLive { get; private set; }
        public (int x, int y) Position { get; private set; }        

        public Life(bool status, (int x, int y) position)
        {
            this.IsLive = status;
            this.Position = position;
        }

        private static Dictionary<bool, bool[]> _gameRules = new Dictionary<bool, bool[]>()
        {
            { true,  new bool[] { false, false, true, true, false, false, false, false, false } },
            { false, new bool[] { false, false, false,true, false, false, false, false, false } },
        };

        public bool NextCycle(MapContext mcx)
        {
            int live_count = mcx.SearchLivesAroundMe().Sum(x => (x.IsLive)?(1):(0));

            return this.IsLive = _gameRules[this.IsLive][live_count];
        }
    }

    public class MapContext
    {
        private bool[,] _world;
        private Life _target;

        public MapContext(bool[,] world, Life target)
        {
            this._world = world;
            this._target = target;
        }

        public bool TrySeeAround(int rel_x, int rel_y, out Life cell)
        {
            // 由環境決定: 只能看到周圍一格的狀態
            if (rel_x < -1 || rel_x > 1) throw new ArgumentOutOfRangeException();
            if (rel_y < -1 || rel_y > 1) throw new ArgumentOutOfRangeException();

            try
            {
                int x = this._target.Position.x + rel_x;
                int y = this._target.Position.y + rel_y;

                cell = new Life(this._world[x, y], (x, y));
                return true;
            }
            catch(IndexOutOfRangeException)
            {
                cell = null;
                return false;
            }
        }

        public IEnumerable<Life> SearchLivesAroundMe()
        {
            Life curlife = null;

            if (this.TrySeeAround(-1, -1, out curlife)) { yield return curlife; }    // 左上
            if (this.TrySeeAround( 0, -1, out curlife)) { yield return curlife; }    // 中上
            if (this.TrySeeAround( 1, -1, out curlife)) { yield return curlife; }    // 右上

            if (this.TrySeeAround(-1,  0, out curlife)) { yield return curlife; }    // 左中
            //
            if (this.TrySeeAround( 1,  0, out curlife)) { yield return curlife; }    // 右中

            if (this.TrySeeAround(-1,  1, out curlife)) { yield return curlife; }    // 左下
            if (this.TrySeeAround( 0,  1, out curlife)) { yield return curlife; }    // 中下
            if (this.TrySeeAround( 1,  1, out curlife)) { yield return curlife; }    // 右下
        }
    }
}
