using System;
using System.Collections.Generic;
using System.Threading;

namespace GameHost1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[,] matrix = new bool[50, 50];
            bool[,] area = new bool[3, 3];

            Init(matrix, 10);

            for (int count = 0; count < 5000; count++)
            {
                int live_count = 0;
                Thread.Sleep(200);

                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var cell = new Life(matrix[x, y], (x, y));
                        var mapcx = new MapContext(matrix, cell);
                        matrix[x, y] = cell.NextCycle(mapcx);

                        Console.Write(matrix[x, y]? '★' : '☆');
                        if (matrix[x, y]) live_count++;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"total lives: {live_count}, round: {count} / 5000...");
            }
        }


        static void Init(bool[,] matrix, int rate = 20)
        {
            Random rnd = new Random();

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[x, y] = (rnd.Next(100) < rate);
                }
            }
        }
    }


    internal class Life
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
            int live_count = 0;

            foreach(var n in mcx.SearchLivesAroundMe())
            {
                if (n == null) continue;
                if (n.IsLive) live_count++;
            }

            return _gameRules[this.IsLive][live_count];
        }
    }

    internal class MapContext
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
