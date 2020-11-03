using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace GameHost1
{
    public interface IWorld
    {
        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until);
    }

    public interface ILife
    {
        public bool IsAlive { get; }
    }


    public class Program
    {
        static void Main(string[] args)
        {
            bool[,] matrix = new bool[50, 20];
            int[,] frames = new int[50, 20];
            Init(matrix, frames, 100, 20);

            IWorld world = new World(matrix, frames, 100);

            int count = 0;

            TimeSpan until = TimeSpan.FromMinutes(10);
            Stopwatch timer = new Stopwatch();

            timer.Restart();
            foreach(var frame in world.Running(until))
            {
                count++;
                int live_count = 0;
                Console.SetCursorPosition(0, 0);

                var current_matrix = frame.matrix;
                var time = frame.time;

                for (int y = 0; y < current_matrix.GetLength(1); y++)
                {
                    for (int x = 0; x < current_matrix.GetLength(0); x++)
                    {
                        var c = current_matrix[x, y];
                        if (c.IsAlive) live_count++;
                        Console.Write(c.IsAlive ? '★' : '☆');
                    }
                    Console.WriteLine();
                }

                if (time > timer.Elapsed) Thread.Sleep(time - timer.Elapsed);
                Console.WriteLine($"total lives: {live_count}, time frame: {time} / {until}...");
            }
        }


        public static bool[,] GetNextGenMatrix(bool[,] matrix)
        {
            int[,] frames = new int[matrix.GetLength(0), matrix.GetLength(1)];
            foreach (var (x, y) in World.ForEachPos<bool>(matrix)) frames[x, y] = 10;

            var world = new World(matrix, frames, 10);

            bool[,] result = new bool[matrix.GetLength(0), matrix.GetLength(1)];
            var lifes = world.Running(TimeSpan.FromSeconds(100)).First().matrix;
            foreach (var (x, y) in World.ForEachPos<ILife>(lifes)) result[x, y] = lifes[x, y].IsAlive;

            return result;
        }

        private static void Init(bool[,] matrix, int[,] frames, int cell_frame = 10, int rate = 20)
        {
            Random rnd = new Random();
            foreach(var (x, y) in World.ForEachPos<bool>(matrix))
            {
                matrix[x, y] = (rnd.Next(100) < rate);
                frames[x, y] = cell_frame;
            }
        }
    }
}
