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
            var world = new World(50, 20);
            world.InitRandomMap(20);

            // TODO: 模擬時間經過取代 gen 迴圈?

            for (int gen = 0; gen < 5000; gen++)
            {
                int live_count = 0;

                Thread.Sleep(200);
                Console.SetCursorPosition(0, 0);

                for (int y = 0; y < world.ColumnNum; y++)
                {
                    for (int x = 0; x < world.RowNum; x++)
                    {
                        var c = world.Cells[x, y];
                        live_count += (c.IsAlive ? 1 : 0);
                        Console.Write(c.IsAlive ? '★' : '☆');
                    }
                    Console.WriteLine();
                }

                world.GetNextGen();
                Console.WriteLine($"total lives: {live_count}, round: {gen} / 5000...");
            }
        }
    }
}
