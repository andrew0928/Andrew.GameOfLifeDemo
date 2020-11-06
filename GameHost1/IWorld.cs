using System;
using System.Collections.Generic;

namespace GameHost1
{
    public interface IWorld
    {
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame);

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until);
    }
}
