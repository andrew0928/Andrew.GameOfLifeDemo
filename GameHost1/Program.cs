using System;
using System.Threading;

namespace GameHost1
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[,] matrix_current = new bool[50, 50];
            bool[,] matrix_next = new bool[50, 50];
            bool[,] matrix_temp;
            
            bool[,] area = new bool[3, 3];

            Init(matrix_current);

            for (int count = 0; count < 5000; count++)
            {
                int live_count = 0;
                Thread.Sleep(200);

                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < matrix_current.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix_current.GetLength(1); x++)
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
                                else if (cx >= matrix_current.GetLength(1)) area[ax, ay] = false;
                                else if (cy >= matrix_current.GetLength(0)) area[ax, ay] = false;
                                else area[ax, ay] = matrix_current[cx, cy];
                            }
                        }

                        matrix_next[x, y] = TimePassRule(area);
                        Console.Write(matrix_next[x, y] ? '★' : '☆');
                        if (matrix_next[x, y]) live_count++;
                    }
                    Console.WriteLine();
                }

                // switch
                matrix_temp = matrix_current;
                matrix_current = matrix_next;
                matrix_next = matrix_temp;

                #if (DEBUG)
                    // debug: clean up [next] map..
                    for (int y = 0; y < matrix_next.GetLength(1); y++) for (int x = 0; x < matrix_next.GetLength(0); x++) matrix_next[x, y] = false;
                #endif

                Console.WriteLine($"total lives: {live_count}, round: {count} / 5000...");
            }
        }


        static void Init(bool[,] matrix)
        {
            Random rnd = new Random();
            int rate = 20;

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[x, y] = (rnd.Next(100) < rate);
                }
            }

            //// 滑翔機 pattern
            //matrix[4, 4] = true;
            //matrix[6, 4] = true;
            //matrix[5, 5] = true;
            //matrix[6, 5] = true;
            //matrix[5, 6] = true;

            //// 信號燈 pattern
            //matrix[10, 5] = true;
            //matrix[11, 5] = true;
            //matrix[12, 5] = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="area">must be bool[3, 3]</param>
        /// <returns></returns>
        static bool TimePassRule(bool[,] area)
        {
            // TODO: fill your code here
            int live_cell = 0;

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (area[x, y] == true)
                    {
                        live_cell++;
                    }
                }
            }

            if (area[1, 1] == true)
            {
                live_cell--;

                if (live_cell < 2)
                {
                    return false;
                }
                else if (live_cell == 2 || live_cell == 3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (live_cell == 3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

// 生命遊戲中，對於任意細胞，規則如下：
// 每個細胞有兩種狀態 - 存活或死亡，每個細胞與以自身為中心的周圍八格細胞產生互動（如圖，黑色為存活，白色為死亡）
// 當前細胞為存活狀態時，當周圍的存活細胞低於2個時（不包含2個），該細胞變成死亡狀態。（模擬生命數量稀少）
// 當前細胞為存活狀態時，當周圍有2個或3個存活細胞時，該細胞保持原樣。
// 當前細胞為存活狀態時，當周圍有超過3個存活細胞時，該細胞變成死亡狀態。（模擬生命數量過多）
// 當前細胞為死亡狀態時，當周圍有3個存活細胞時，該細胞變成存活狀態。（模擬繁殖）
// 可以把最初的細胞結構定義為種子，當所有在種子中的細胞同時被以上規則處理後，可以得到第一代細胞圖。按規則繼續處理當前的細胞圖，可以得到下一代的細胞圖，周而復始。