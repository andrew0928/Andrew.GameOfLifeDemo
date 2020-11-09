using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Tests
{
    internal static class MatrixHelper
    {
        internal static void CompareMatrix(bool[,] source, ILife[,] target)
        {
            if (source == null) throw new ArgumentNullException();
            if (target == null) throw new ArgumentNullException();
            if (source.GetLength(0) != target.GetLength(0)) throw new ArgumentOutOfRangeException();
            if (source.GetLength(1) != target.GetLength(1)) throw new ArgumentOutOfRangeException();

            foreach (var (x, y) in ArrayHelper.ForEachPos<bool>(source))
            //for (int y = 0; y < source.GetLength(1); y++)
            {
                //for (int x = 0; x < source.GetLength(0); x++)
                {
                    if (source[x, y] != target[x, y].IsAlive) throw new ArgumentException();
                }
            }

            return;
        }

        internal static bool[,] _Transform(string[] map)
        {
            bool[,] matrix = new bool[map[0].Length, map.Length];

            int x = 0;
            int y = 0;
            foreach (var line in map)
            {
                foreach (var c in line)
                {
                    matrix[x, y] = (c == '1');
                    x++;
                }
                x = 0;
                y++;
            }

            return matrix;
        }

    }
}
