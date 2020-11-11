using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1
{
    public interface IWorld
    {
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="until">指定最長的執行時間 (模擬的 world 時間)</param>
        /// <param name="realtime">指定是否模擬實際的時間進行速度? true: 是 (1:1), false: 否 (用最快速度模擬)</param>
        /// <returns></returns>
        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until, bool realtime = true);
    }


    public interface ILife
    {
        /// <summary>
        /// 細胞演化次數。Milestone3 如果是 0 則會跳過檢查
        /// </summary>
        public int Generation { get; }
        public bool IsAlive { get; }
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
