using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class World : IWorld
    {
        int width;
        int height;
        ICell[,] matrix = null;

        public World(bool[,] matrix_current)
        {
            //Dump("Input", matrix_current);
            (width, height) = (matrix_current.GetLength(0), matrix_current.GetLength(1));

            matrix = new Cell[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    matrix[x, y] = new Cell(x, y, matrix_current[x, y]);
                }
            }
        }

        public IEnumerable<ICell> GetNeighbors(ICell cell, int redius = 1)
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
                        //Console.WriteLine($"({cy}, {cx})");
                        yield return matrix[cx, cy];
                    }
                }
            }
        }

        public IEnumerable<ICell> Traverse()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return matrix[x, y];
                }
            }
        }
        public bool[,] NextMoment()
        {
            bool[,] output = new bool[width, height];

            foreach (var cell in Traverse())
            {
                output[cell.PosX, cell.PosY] = cell.WillBeAlive;
            }

            return output;
        }
    }
}
