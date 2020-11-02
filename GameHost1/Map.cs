namespace GameHost1
{
    public class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Rate { get; set; } = 20;

        public Cell[,] Matrix { get; set; }

        public Map() { }

        public Map(Cell[,] matrix) 
        {
            this.Width = matrix.GetLength(0);
            this.Height = matrix.GetLength(1);
            this.Matrix = matrix;
            SetPartners();
        }

        public void Init(int w, int h)
        {
            this.Width = w;
            this.Height = h;
            this.Matrix = new Cell[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Matrix[x, y] = new Cell(Rate);
                }
            }

            SetPartners();
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

                            if (cx < 0) partners[ax, ay].Status = false;
                            else if (cy < 0) partners[ax, ay].Status = false;
                            else if (cx >= Matrix.GetLength(0)) partners[ax, ay].Status = false;
                            else if (cy >= Matrix.GetLength(1)) partners[ax, ay].Status = false;
                            else partners[ax, ay] = Matrix[cx, cy];
                        }
                    }
                    Matrix[x, y].Partners = partners;
                }
            }
        }

        public Map GetNextGeneration() 
        {
            var next = new Map();
            next.Init(this.Width, this.Height);

            for (int y = 0; y < next.Height; y++)
            {
                for (int x = 0; x < next.Width; x++)
                {
                    next.Matrix[x, y].Status = Matrix[x, y].IsAlive();
                }
            }
            return next;
        }

        public bool[,] ConvertToBoolMatrix() 
        {
            var result = new bool[Width, Height];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    result[x, y] = Matrix[x, y].Status;
                }
            }
            return result;
        }
    }
}
