using System;
using System.Collections.Generic;
using System.Threading;

namespace GameHost1
{
    public class Map : IWorld
    {
        private int Width { get; set; }
        private int Height { get; set; }
        private Cell[,] Matrix { get; set; }
        private int Interval { get; set; }

        public Map() { }

        public Map(int width, int depth)
        {
            this.Width = width;
            this.Height = depth;
        }

        private Map(Cell[,] matrix)
        {
            this.Width = matrix.GetLength(0);
            this.Height = matrix.GetLength(1);
            this.Matrix = matrix;
            SetPartners();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="init_matrix">決定這個世界每個細胞的初始狀態</param>
        /// <param name="init_cell_frame">則代表每個細胞間隔多久會進到下個世代</param>
        /// <param name="init_cell_start_frame">代表細胞在世界啟動後多久會開始活動</param>
        /// <param name="world_frame">這世界多久會刷新一次</param>
        /// <returns></returns>
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            if (init_matrix.GetLength(0) != this.Width
                || init_matrix.GetLength(1) != this.Height
                || init_cell_frame.GetLength(0) != this.Width
                || init_cell_frame.GetLength(1) != this.Height
                || init_cell_start_frame.GetLength(0) != this.Width
                || init_cell_start_frame.GetLength(1) != this.Height)
                return false;

            this.Width = init_matrix.GetLength(0);
            this.Height = init_matrix.GetLength(1);
            this.Matrix = new Cell[Width, Height];
            this.Interval = world_frame;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var isAlive = init_matrix[x, y];
                    var interval = init_cell_frame[x, y];
                    var startTime = init_cell_start_frame[x, y];
                    Matrix[x, y] = new Cell(isAlive, interval, startTime);
                }
            }

            SetPartners();

            return true;
        }

        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until)
        {
            var startTime = DateTime.Now;
            foreach (var cell in Matrix)
            {
                cell.AwakeAfterSleep();
            }

            while (true)
            {
                var elapsed = DateTime.Now - startTime;
                if (elapsed >= until) break;

                yield return (elapsed, Matrix);


                var next = this.GetNextGeneration();
                //if (CampareMap(next))
                //    break;
                this.Matrix = next;

                Thread.Sleep(Interval);
            }
        }

        private void SetPartners()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var partners = new Cell[3, 3];
                    for (int ay = 0; ay < 3; ay++)
                    {
                        for (int ax = 0; ax < 3; ax++)
                        {
                            int cx = x - 1 + ax;
                            int cy = y - 1 + ay;

                            partners[ax, ay] = new Cell();

                            if (cx < 0) partners[ax, ay].IsAlive = false;
                            else if (cy < 0) partners[ax, ay].IsAlive = false;
                            else if (cx >= Matrix.GetLength(0)) partners[ax, ay].IsAlive = false;
                            else if (cy >= Matrix.GetLength(1)) partners[ax, ay].IsAlive = false;
                            else partners[ax, ay].IsAlive = Matrix[cx, cy].IsAlive;
                        }
                    }
                    Matrix[x, y].Partners = partners;
                }
            }
        }

        private Cell[,] GetNextGeneration()
        {
            var next = new Cell[this.Width, this.Height];

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    next[x, y] = new Cell
                    {
                        IsAlive = Matrix[x, y].IsAlive
                    };
                }
            }
            return next;
        }

        private bool CampareMap(Cell[,] target) 
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (target[x, y].IsAlive != this.Matrix[x, y].IsAlive)
                        return false;
                }
            }
            return true;
        }
    }
}
