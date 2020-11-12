#define ENABLE_RUNNING_RECORDING
using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class World : IWorld
    {
        int width;
        int height;
        ICell[,] matrix = null;

        int worldFrame;

        public World(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            matrix = new Cell[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    matrix[x, y] = new Cell(x, y, init_matrix[x, y], init_cell_frame[x, y], init_cell_start_frame[x, y]);
                }
            }

            this.worldFrame = world_frame;

            return true;
        }

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until, bool realtime = true)
        {
            var current = new TimeSpan();
            //var god = new God();

            yield return (current, this.CurrentGeneration());

            while (current < until)
            {
                current = current.Add(TimeSpan.FromMilliseconds(this.worldFrame));

                foreach (var cell in Traverse())
                {
                    cell.GetAlongWith(GetNeighbors(cell));
                    //god.Dump($"({cell.PosY}, {cell.PosX})", this.NextGeneration());
                }

                this.Refresh();

                yield return (current, this.CurrentGeneration());
            }
        }

        private IEnumerable<ICell> GetNeighbors(ICell cell, int redius = 1)
        {
            //Console.WriteLine($"current: ({cell.PosY}, {cell.PosX})");
            for (int ay = -redius; ay <= redius; ay++)
            {
                for (int ax = -redius; ax <= redius; ax++)
                {
                    var cx = cell.PosX + ax;
                    var cy = cell.PosY + ay;

                    if (cx == cell.PosX && cy == cell.PosY)
                    { }
                    else if (cy < 0 || cy > height - 1)
                    { }
                    else if (cx < 0 || cx > width - 1)
                    { }
                    else
                    {
                        yield return matrix[cx, cy];
                    }
                }
            }
        }

        private IEnumerable<ICell> Traverse()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return matrix[x, y];
                }
            }
        }

        private ILife[,] CurrentGeneration()
        {
            ILife[,] output = new ILife[width, height];

            foreach (var cell in Traverse())
            {
                output[cell.PosX, cell.PosY] = new Life
                {
                    Generation = cell.Generation,
                    IsAlive = cell.IsAlive
                };
            }

            return output;
        }

        private void Refresh()
        {
            foreach (var cell in Traverse())
            {
                cell.NextGeneration();
            }
        }
    }
}
