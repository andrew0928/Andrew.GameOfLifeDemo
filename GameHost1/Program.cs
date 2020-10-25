using System;
using System.Threading;

namespace GameHost1
{
    public class Program
    {
        static void Main(string[] args)
        {
            RunGameOfLife();
        }


        private static void RunGameOfLife()
        {
            Map map = new Map() 
            {
                Width = 50,
                Height = 20,
                Rate = 20
            };
            map.Init();

            for (int count = 0; count < 5000; count++)
            {
                int live_count = 0;

                Thread.Sleep(200);
                Console.SetCursorPosition(0, 0);

                var matrix = map.Matrix;

                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        var c = matrix[x, y];
                        live_count += (c.Status ? 1 : 0);
                        Console.Write(c.Status ? '★' : '☆');
                    }
                    Console.WriteLine();
                }

                map.Matrix = map.ConvertToCellMatrix(GetNextGenMatrix(map.ConvertToBoolMatrix()));
                Console.WriteLine($"total lives: {live_count}, round: {count} / 5000...");
            }
        }


        public static bool[,] GetNextGenMatrix(bool[,] matrix_current)
        {
            bool[,] matrix_next = new bool[matrix_current.GetLength(0), matrix_current.GetLength(1)];
            Cell[,] cells = new Cell[3, 3];

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

                            cells[ax, ay] = new Cell();

                            if (cx < 0) cells[ax, ay].Status = false;
                            else if (cy < 0) cells[ax, ay].Status = false;
                            else if (cx >= matrix_current.GetLength(0)) cells[ax, ay].Status = false;
                            else if (cy >= matrix_current.GetLength(1)) cells[ax, ay].Status = false;
                            else cells[ax, ay].Status = matrix_current[cx, cy];
                        }
                    }

                    var target = cells[1, 1];
                    target.Partners = cells;

                    matrix_next[x, y] = TimePassRule(target);
                }
            }

            return matrix_next;
        }


        public static bool TimePassRule(Cell cell)
        {
            return cell.IsAlive();
        }
    }
}
