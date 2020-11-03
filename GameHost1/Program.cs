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
            Map map = new Map();
            map.Init(50, 20);

            for (int count = 0; count < 5000; count++)
            {
                int liveCount = 0;

                Thread.Sleep(500);
                Console.SetCursorPosition(0, 0);

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        var c = map.Matrix[x, y];
                        liveCount += (c.Status ? 1 : 0);
                        Console.Write(c.Status ? '★' : '☆');
                    }
                    Console.WriteLine();
                }

                map = map.GetNextGeneration();

                Console.WriteLine($"total lives: {liveCount}, round: {count} / 5000...");
            }
        }
    }
}
