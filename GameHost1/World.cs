using System;
using System.Collections.Generic;

namespace GameHost1
{
    public class World : IWorld
    {
        public int Width { get; set; }
        public int Depth { get; set; }
        public ILife[,] Matrix { get; set; }
        public int[,] CellFrame { get; set; }
        public int WorldFrame { get; set; }

        public World(int width, int depth)
        {
            this.Width = width;
            this.Depth = depth;
        }

        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            if (init_matrix.GetLength(0) != Width || init_matrix.GetLength(1) != Depth) throw new ArgumentOutOfRangeException();
            Matrix = new ILife[Width, Depth];
            for (int y = 0; y < Depth; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Matrix[x, y] = new Life(init_matrix[x, y]);
                }
            }
            CellFrame = init_cell_frame;
            WorldFrame = world_frame;
            return true;
        }
        /// <summary>
        /// 根據經過的時間長度和定義過的 frame, 回傳每個時間點的 life matrix
        /// </summary>
        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until)
        {
            // TODO: update cells according to its env & frame
            // TODO: update world according to its matrix & frame
            throw new NotImplementedException();
        }
    }
}
