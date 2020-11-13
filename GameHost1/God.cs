#define ENABLE_RUNNING_RECORDING
using System;

namespace GameHost1
{
    public class God
    {
        public void Dump(string message, ILife[,] matrix)
        {
            Console.WriteLine(message);
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    var c = matrix[x, y];
                    Console.Write(c.IsAlive ? '★' : '☆');
                }
                Console.WriteLine();
            }
        }
    }
}
