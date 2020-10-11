using System;
using System.Threading;

namespace GameHost1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[,] matrix = new bool[10, 10];
            bool[,] area = new bool[3, 3];

            Init(matrix);

            for (int count = 0; count < 5000; count++)
            {
                int live_count = 0;
                Thread.Sleep(2000);

                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    for (int x = 0; x < matrix.GetLength(0); x++)
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
                                else if (cx >= matrix.GetLength(1)) area[ax, ay] = false;
                                else if (cy >= matrix.GetLength(0)) area[ax, ay] = false;
                                else area[ax, ay] = matrix[cx, cy];
                            }
                        }
                        matrix[x, y] = TimePassRule(area);
                        Console.Write(matrix[x, y] ? '★' : '☆');
                        if (matrix[x, y]) live_count++;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"total lives: {live_count}, round: {count} / 5000...");
            }
        }


        static void Init(bool[,] matrix)
        {
            Random rnd = new Random();
            int rate = 20;

            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    matrix[x, y] = (rnd.Next(100) < rate);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="area">must be bool[3, 3]</param>
        /// <returns></returns>
        static bool TimePassRule(bool[,] area)
        {
            // TODO: fill your code here
            // Find center
            var center_x = area.GetLength(0) / 2;
            var center_y = area.GetLength(1) / 2;
            var center = area[center_x, center_y];

            // Calculate lives & deaths around it
            var lives = 0;
            for (int ay = 0; ay < area.GetLength(1); ay++)
            {
                for (int ax = 0; ax < area.GetLength(0); ax++)
                {
                    Console.Write("["+ax+",");
                    Console.Write(ay+"]");
                    if (ax == center_x && ay == center_y) continue;
                    if (area[ax, ay] == true) lives++;
                }
            }

            if (center == true && lives >= 2 && lives <= 3) return true;
            if (center == false && lives == 3) return true;

            return false;
        }
    }
}
