using System;
using System.Collections.Generic;
using System.Threading;

namespace GameHost1
{
    public class Program
    {
        public static bool TimePassRule(bool[,] area)
        {
            var value = 0;
            for (int y = 0; y < area.GetLength(1); y++)
            {
                for (int x = 0; x < area.GetLength(0); x++)
                {
                    value += area[x, y] ? 1 : 0;
                }
            }
            value -= (area[1, 1] ? 1 : 0);
            //alive
            if (area[1, 1])
            {
                return (value < 2 || value > 3) ? false : true;
            }
            else
            {
                return (value == 3) ? true : false;
            }
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

                for (int y = 0; y < matrix.GetLength(1); y++)
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
