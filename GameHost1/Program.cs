using System;
using System.Collections.Generic;
using System.Threading;

namespace GameHost1
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="area">must be bool[3, 3]</param>
        /// <returns></returns>
        static bool TimePassRule(bool[,] area)
        {
            // TODO: fill your code here
            return area[1, 1];
        }








        static void Main(string[] args)
        {
            #region run test case
            RunUnitTest(
                "穩定狀態: 板凳(1)", 5,
                new List<string[]>()
                {
                    new string[]
                    {
                        "11",
                        "11",
                    }
                });

            RunUnitTest(
                "穩定狀態: 麵包(1)", 5,
                new List<string[]>()
                {
                    new string[]
                    {
                        "000000",
                        "001100",
                        "010010",
                        "001010",
                        "000100",
                        "000000",
                    }
                });

            RunUnitTest(
                "穩定狀態: 蜂巢(1)", 5,
                new List<string[]>()
                {
                    new string[]
                    {
                        "000000",
                        "001100",
                        "010010",
                        "001100",
                        "000000",
                    }
                });


            RunUnitTest(
                "震盪狀態: 信號燈(2)", 5,
                new List<string[]>()
                {
                    new string[] {
                        "00000",
                        "00000",
                        "01110",
                        "00000",
                        "00000"
                    },
                    new string[] {
                        "00000",
                        "00100",
                        "00100",
                        "00100",
                        "00000"
                    },
                });

            // 震盪狀態: 蟾蜍(2)
            RunUnitTest(
                "震盪狀態: 蟾蜍(2)", 5,
                new List<string[]>()
                {
                    new string[] {
                        "000000",
                        "000000",
                        "001110",
                        "011100",
                        "000000",
                        "000000",
                    },
                    new string[] {
                        "000000",
                        "000100",
                        "010010",
                        "010010",
                        "001000",
                        "000000",
                    },
                });

            RunUnitTest(
                "震盪狀態: 烽火 (2)", 5,
                new List<string[]>()
                {
                    new string[] {
                        "000000",
                        "011000",
                        "010000",
                        "000010",
                        "000110",
                        "000000",
                    },
                    new string[] {
                        "000000",
                        "011000",
                        "011000",
                        "000110",
                        "000110",
                        "000000",
                    },
                });

            RunUnitTest(
                "會移動的振盪狀態: 滑翔機 (4)", 1,
                new List<string[]>()
                {
                    new string[] {
                        "000000",
                        "010000",
                        "001100",
                        "011000",
                        "000000",
                        "000000",
                    },
                    new string[] {
                        "000000",
                        "001000",
                        "000100",
                        "011100",
                        "000000",
                        "000000",
                    },
                    new string[] {
                        "000000",
                        "000000",
                        "010100",
                        "001100",
                        "001000",
                        "000000",
                    },
                    new string[] {
                        "000000",
                        "000000",
                        "000100",
                        "010100",
                        "001100",
                        "000000",
                    },
                });

            Console.WriteLine("PRESS [ENTER] to continue...");
            Console.ReadLine();
            #endregion

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


        private static bool[,] GetNextGenMatrix(bool[,] matrix_current)
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








        private static void RunUnitTest(string pattern_name, int repeat_count, List<string[]> testcase)
        {
            Console.Write($"測試案例: {pattern_name} ...");

            try
            {
                bool[,] input_matrix = _Transform(testcase[0]);
                for (int i = 1; i < testcase.Count * repeat_count; i++)
                {
                    bool[,] expected_matrix = _Transform(testcase[i % testcase.Count]);
                    bool[,] actual_matrix = GetNextGenMatrix(input_matrix);

                    // compare
                    CompareMatrix(expected_matrix, actual_matrix);
                    input_matrix = actual_matrix;
                }
            }
            catch
            {
                Console.WriteLine(" FAILED.");
                return;
            }

            Console.WriteLine(" PASSED.");
        }

        private static void CompareMatrix(bool[,] source, bool[,] target)
        {
            if (source == null) throw new ArgumentNullException();
            if (target == null) throw new ArgumentNullException();
            if (source.GetLength(0) != target.GetLength(0)) throw new ArgumentOutOfRangeException();
            if (source.GetLength(1) != target.GetLength(1)) throw new ArgumentOutOfRangeException();

            for (int y = 0; y < source.GetLength(1); y++)
            {
                for (int x = 0; x < source.GetLength(0); x++)
                {
                    if (source[x, y] != target[x, y]) throw new ArgumentException();
                }
            }

            return;
        }

        private static bool[,] _Transform(string[] map)
        {
            bool[,] matrix = new bool[map[0].Length, map.Length];

            int x = 0;
            int y = 0;
            foreach(var line in map)
            {
                foreach(var c in line)
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
