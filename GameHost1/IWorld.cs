using System;
using System.Collections.Generic;

namespace GameHost1
{
    public interface IWorld
    {
        public int Width { get; set; }
        public int Depth { get; set; }
        public ILife[,] Matrix { get; set; }
        public int[,] CellFrame { get; set; }
        public int WorldFrame { get; set; }
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame);

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until);
    }
}
