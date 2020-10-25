using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1
{
    public class Map
    {
        public bool[,] Matrix { get; set; }

        public void Init(int w, int h, int rate = 20)
        {
            Matrix = new bool[w, h];
            Random rnd = new Random();
            for (int y = 0; y < Matrix.GetLength(1); y++)
            {
                for (int x = 0; x < Matrix.GetLength(0); x++)
                {
                    Matrix[x, y] = (rnd.Next(100) < rate);
                }
            }
        }
    }
}
