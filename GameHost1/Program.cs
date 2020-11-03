using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace GameHost1
{
    public interface IWorld
    {
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame);

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until);
    }

    public interface ILife
    {
        public bool IsAlive { get; }
    }


    public class Program
    {
        public static IWorld CreateWorld(int width, int depth)
        {
            //
            //  ToDo: fill your code HERE.
            //
            throw new NotImplementedException();
        }

        private static void Init(bool[,] matrix, int[,] frames, int cell_frame = 10, int rate = 20)
        {
            Random rnd = new Random();
            foreach (var (x, y) in Program.ForEachPos<bool>(matrix))
            {
                matrix[x, y] = (rnd.Next(100) < rate);
                frames[x, y] = cell_frame;
            }
        }

        public static void Main(string[] args)
        {
            IWorld world = CreateWorld(50, 20);

            #region Init the world...

            bool[,] matrix = new bool[50, 20];
            int[,] frames = new int[50, 20];
            int[,] start_frames = new int[50, 20];
            Init(matrix, frames, 100, 20);

            world.Init(matrix, frames, start_frames, 100);
            
            #endregion



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





        #region utility method(s)...
        public static IEnumerable<(int x, int y)> ForEachPos<T>(T[,] input)
        {
            for(int y = 0; y < input.GetLength(1); y++)
            {
                for (int x = 0; x < input.GetLength(0); x++)
                {
                    yield return (x, y);
                }
            }
        }
        #endregion
    }
}
