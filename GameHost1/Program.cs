using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GameHost1
{
    public class Program
    {
        /// <summary>
        /// 周遭有幾個存活細胞可維持存活
        /// </summary>
        private static readonly int[] KeepAliveCount = {2, 3};
        
        /// <summary>
        /// 周遭有幾個存活細胞可重生
        /// </summary>
        private const int RebornCount = 3;
        
        /// <summary>
        /// 是否存活
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private static bool IsAlive(bool[,] area) => area[1, 1];

        /// <summary>
        /// 取得周遭存活數量
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private static int GetAroundAliveCount(bool[,] area)
        {
            int count = 0;
            foreach (var status in area)
            {
                if (status) count++;
            }

            if (area[1, 1]) count -= 1;
            return count;
        }
        
        public static bool TimePassRule(bool[,] area)
        {
            int aliveCount = GetAroundAliveCount(area);
            return IsAlive(area)
                ? KeepAliveCount.Contains(aliveCount)
                : RebornCount == aliveCount;
        }



        static void Main(string[] args)
        {
            RunGameOfLife();
        }


        private static void RunGameOfLife()
        { 
            bool[,] matrix = new bool[50, 20];

            Init(matrix, 20);
            for (int count = 0; count < 5000; count++)
            {
                int live_count = 0;

                Thread.Sleep(200);
                Console.SetCursorPosition(0, 0);

                for(int y = 0; y < matrix.GetLength(1); y++)
                {
                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        var c = matrix[x, y];
                        live_count += (c ? 1 : 0);
                        Console.Write(c ? '★' : '☆');
                    }
                    Console.WriteLine();
                }

                matrix = GetNextGenMatrix(matrix);
                Console.WriteLine($"total lives: {live_count}, round: {count} / 5000...");
            }
        }


        public static bool[,] GetNextGenMatrix(bool[,] matrix_current)
        {
            bool[,] matrix_next = new bool[matrix_current.GetLength(0), matrix_current.GetLength(1)];
            bool[,] area = new bool[3, 3];

            for (int y = 0; y < matrix_current.GetLength(1); y++)
            {
                for (int x = 0; x < matrix_current.GetLength(0); x++)
                {
                    // clone area
                    for (int ay = 0; ay < 3; ay++)
                    {
                        for (int ax = 0; ax < 3; ax++)
                        {
                            int cx = x - 1 + ax;
                            int cy = y - 1 + ay;

                            if (cx < 0) area[ax, ay] = false;
                            else if (cy < 0) area[ax, ay] = false;
                            else if (cx >= matrix_current.GetLength(0)) area[ax, ay] = false;
                            else if (cy >= matrix_current.GetLength(1)) area[ax, ay] = false;
                            else area[ax, ay] = matrix_current[cx, cy];
                        }
                    }

                    matrix_next[x, y] = TimePassRule(area);
                }
            }

            return matrix_next;
        }

        private static void Init(bool[,] matrix, int rate = 20)
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
}
