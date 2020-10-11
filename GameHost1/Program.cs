using System;
using System.Collections.Generic;
using System.Threading;

namespace GameHost1
{
    public class Program
    {
        
        public static bool TimePassRule(bool[,] area)
        {
            // TODO: fill your code here
            return area[1, 1];
        }



        static void Main(string[] args)
        {
            RunGameOfLife();
        }


        private static void RunGameOfLife()
        { 
            bool[,] matrix = new bool[50, 20];

            Init(matrix, 20);
            for (int count = 0; count < 5000; count++)
            {
                int live_count = 0;

                Thread.Sleep(200);
                Console.SetCursorPosition(0, 0);

                for(int y = 0; y < matrix.GetLength(1); y++)
                {
                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        var c = matrix[x, y];
                        live_count += (c ? 1 : 0);
                        Console.Write(c ? '★' : '☆');
                    }
                    Console.WriteLine();
                }

                matrix = GetNextGenMatrix(matrix);
                Console.WriteLine($"total lives: {live_count}, round: {count} / 5000...");
            }
        }


        public static bool[,] GetNextGenMatrix(bool[,] matrix_current)
        {
            bool[,] matrix_next = new bool[matrix_current.GetLength(0), matrix_current.GetLength(1)];
            bool[,] area = new bool[3, 3];

            for (int y = 0; y < matrix_current.GetLength(1); y++)
            {
                for (int x = 0; x < matrix_current.GetLength(0); x++)
                {
                    // clone area
                    for (int ay = 0; ay < 3; ay++)
                    {
                        for (int ax = 0; ax < 3; ax++)
                        {
                            int cx = x - 1 + ax;
                            int cy = y - 1 + ay;

                            if (cx < 0) area[ax, ay] = false;
                            else if (cy < 0) area[ax, ay] = false;
                            else if (cx >= matrix_current.GetLength(0)) area[ax, ay] = false;
                            else if (cy >= matrix_current.GetLength(1)) area[ax, ay] = false;
                            else area[ax, ay] = matrix_current[cx, cy];
                        }
                    }

                    matrix_next[x, y] = TimePassRule(area);
                }
            }

            return matrix_next;
        }

        /// <summary>
        /// 1. 每個細胞有兩種狀態 - 存活或死亡，每個細胞與以自身為中心的周圍八格細胞產生互動（如圖，黑色為存活，白色為死亡）
        /// 2. 當前細胞為存活狀態時，當周圍的存活細胞低於2個時（不包含2個），該細胞變成死亡狀態。（模擬生命數量稀少）
        /// 3. 當前細胞為存活狀態時，當周圍有2個或3個存活細胞時，該細胞保持原樣。
        /// 4. 當前細胞為存活狀態時，當周圍有超過3個存活細胞時，該細胞變成死亡狀態。（模擬生命數量過多）
        /// 5. 當前細胞為死亡狀態時，當周圍有3個存活細胞時，該細胞變成存活狀態。（模擬繁殖）
        /// 6. 可以把最初的細胞結構定義為種子，當所有在種子中的細胞同時被以上規則處理後，可以得到第一代細胞圖。按規則繼續處理當前的細胞圖，可以得到下一代的細胞圖，周而復始。 
        /// </summary>
        /// <param name="area">must be bool[3, 3]</param>
        /// <returns></returns>
        static bool TimePassRule(bool[,] area)
        {
            var center = area[1, 1];
        private static void Init(bool[,] matrix, int rate = 20)
        {
            Random rnd = new Random();
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    matrix[x, y] = (rnd.Next(100) < rate);
                }
            }
        }


            int alive = 0;

            for (int i = 0; i < area.GetLength(0); i++)
                for (int k = 0; k < area.GetLength(1); k++)
                    if (area[i, k]) alive++;

            if (center)
            {
                alive--;
                if (alive < 2 || alive > 3)
                    center = !center;
            }
            else
            {
                if (alive == 3)
                    center = !center;
            }
            return center;
        }
    }
}
