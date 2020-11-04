using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public class World : IWorld
    {
        private bool _alreadyInitialize = false;
        
        /// <summary>
        /// msec
        /// </summary>
        private int _worldFrame;

        /// <summary>
        /// msec
        /// </summary>
        private int _currentWorldFrame = 0;
        

        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            // 沒考慮多執行緒 Init 的情境
            if (_alreadyInitialize)
            {
                return false;
            }


            
            
            _worldFrame = world_frame;


            _alreadyInitialize = true;

            return true;
        }

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until)
        {
            // TODO: 每一次 run 都去找有哪些 life 需要演化。


            throw new NotImplementedException();
        }



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
