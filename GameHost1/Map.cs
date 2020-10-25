using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Rate { get; set; }


        public Cell[,] Matrix { get; set; }

        public void Init()
        {
            Matrix = new Cell[Width, Height];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Matrix[x, y] = new Cell();
                    Matrix[x, y].Init(Rate);
                }
            }
        }

        public bool[,] ConvertToBoolMatrix() 
        {
            var result = new bool[Width, Height];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    result[x, y] = Matrix[x, y].Status;
                }
            }
            return result;
        }

        public Cell[,] ConvertToCellMatrix(bool[,] matrix)
        {
            var result = new Cell[matrix.GetLength(0), matrix.GetLength(1)];
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    result[x, y] = new Cell
                    {
                        Status = matrix[x, y]
                    };
                }
            }
            return result;
        }
    }
}
