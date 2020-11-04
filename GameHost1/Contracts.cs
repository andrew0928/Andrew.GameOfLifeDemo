using System;
using System.Collections.Generic;
using System.Text;

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

    public interface IRunningObject
    {
        public IEnumerable<int> AsTimePass();
    }

    public static class ArrayHelper
    {
        // utility, 簡化到處都出現的雙層迴圈。只會循序取出 2D 陣列中所有的 (x, y) 座標組合
        public static IEnumerable<(int x, int y)> ForEachPos<T>(T[,] array)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                for (int x = 0; x < array.GetLength(0); x++)
                {
                    yield return (x, y);
                }
            }
        }
    }
}
