#define ENABLE_RUNNING_RECORDING
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace GameHost1
{


    public class Program
    {
        public static IWorld CreateWorld(int width, int depth)
        {
            return new World(width, depth);
        }

        private static void Init(bool[,] matrix, int[,] frames, int cell_frame = 10, int rate = 20)
        {
            Random rnd = new Random();
            foreach (var (x, y) in ArrayHelper.ForEachPos<bool>(matrix))
            {
                matrix[x, y] = (rnd.Next(100) < rate);
                frames[x, y] = cell_frame;
            }
        }


        public static void Main(string[] args)
        {

            // 是否用 realtime mode 執行模擬?
            TimeSpan until = TimeSpan.FromMinutes(10);



#if (ENABLE_RUNNING_RECORDING)
            const int width = 5;
            const int depth = 5;
            const int world_frame = 100;
            const bool _enable_running_recording = true;
            const bool realtime = true;
            const bool display = true;

            IWorld world = CreateWorld(width, depth);

            bool[,] matrix = new bool[width, depth]
            {
                { false, false, false, false, false },
                { false, true , true , true , false },
                { false, true , true , true , false },
                { false, true , true , true , false },
                { false, false, false, false, false }
            };
            int[,] frames = new int[width, depth]
            {
                { 30, 30, 30, 30, 30 },
                { 30, 30, 70, 30, 30 },
                { 30, 70, 70, 70, 30 },
                { 30, 30, 70, 30, 30 },
                { 30, 30, 30, 30, 30 },
            };
            int[,] start_frames = new int[width, depth]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
            };

#else
            const int width = 50;
            const int depth = 20;
            const int world_frame = 100;
            const bool _enable_running_recording = false;
            const bool realtime = true;
            const bool display = true;

            IWorld world = CreateWorld(width, depth);

            bool[,] matrix = new bool[width, depth];
            int[,] frames = new int[width, depth];
            int[,] start_frames = new int[width, depth];
            Init(matrix, frames, world_frame, 20);
#endif

            world.Init(matrix, frames, start_frames, world_frame);

            if (_enable_running_recording)
            {
                File.Delete("running-settings.json");
                File.Delete("running-logs.json");

                File.AppendAllText(
                    "running-settings.json",
                    JsonConvert.SerializeObject(new
                    {
                        InitMapFrame = world_frame,
                        InitMap = matrix,
                        InitFrames = frames,
                        InitStarts = start_frames
                    }) + "\n");
            }



            int count = 0;
            Console.CursorVisible = false;

            Stopwatch realtime_timer = new Stopwatch();
            realtime_timer.Restart();
            foreach (var frame in world.Running(until, realtime))
            {
                count++;
                int live_count = 0;
                Console.SetCursorPosition(0, 0);

                var current_matrix = frame.matrix;
                var time = frame.time;

                if (display)
                {
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
                }

                Console.Write("".PadRight(150, ' '));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine($"total lives: {live_count}, time frame: {time} / {until}, speed up: {time.TotalMilliseconds / realtime_timer.ElapsedMilliseconds:0.##}X                 ");


                if (_enable_running_recording)
                {
                    File.AppendAllText(
                        "running-logs.json",
                        JsonConvert.SerializeObject(new
                        {
                            Time = (int)frame.time.TotalMilliseconds,
                            Maps = frame.matrix
                        } + "\n",
                        Formatting.None,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }
                    ));
                    //Thread.Sleep(2000);
                    Console.ReadLine();
                }
            }
        }

    }
}
