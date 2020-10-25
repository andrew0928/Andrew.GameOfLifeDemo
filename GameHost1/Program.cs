using System;
using System.Threading;

namespace GameHost1
{
    public class Program
    {
        static void Main(string[] args)
        {
            RunGameOfLife();
        }

        private static void RunGameOfLife()
        {
            Map map = new Map(50, 20);
            map.Init();

            for (int count = 0; count < 5000; count++)
            {
                int live_count = 0;

                Thread.Sleep(200);
                Console.SetCursorPosition(0, 0);

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        var c = map.Matrix[x, y];
                        live_count += (c.Status ? 1 : 0);
                        Console.Write(c.Status ? '★' : '☆');
                    }
                    Console.WriteLine();
                }

                bool[,] currentMatrix = map.ConvertToBoolMatrix();
                bool[,] nextMatrix = GetNextGenMatrix(currentMatrix);
                map.Init(nextMatrix);

                Console.WriteLine($"total lives: {live_count}, round: {count} / 5000...");
            }
        }


        public static bool[,] GetNextGenMatrix(bool[,] matrix)
        {
            var map = new Map();
            map.Init(matrix);
            return map.GetNextGeneration();
        }
    }
}
