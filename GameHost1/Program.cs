using System;
using System.Threading;

namespace GameHost1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[,] matrix = new bool[50, 50];

            Init(matrix);

            for (int count = 0; count < 5000; count++)
            {
                Thread.Sleep(100);
                TimePass(matrix);

                Console.Clear();
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    for (int y = 0; y < matrix.GetLength(0); y++)
                    {
                        bool value = matrix[x, y];
                        Console.Write(value ? '★' : '☆');
                    }
                    Console.WriteLine();
                }
            }
        }


        static void Init(bool[,] matrix)
        {
            matrix[1, 1] = true;
            matrix[3, 25] = true;
        }

        static void TimePass(bool[,] matrix)
        {
            // TODO: fill your code here
        }
    }
}
