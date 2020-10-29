using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1
{
    public class World
    {
        public readonly (int width, int depth) Dimation;

        private Life[,] _maps_snapshot;
        private Life.Sensibility[,] _maps_current_life_sense;

        public World(bool[,] init_matrix, out GodPower _god)
        {
            this.Dimation = (init_matrix.GetLength(0), init_matrix.GetLength(1));
            this._maps_current_life_sense = new Life.Sensibility[this.Dimation.width, this.Dimation.depth];
            this._maps_snapshot = new Life[this.Dimation.width, this.Dimation.depth];

            //for (int y = 0; y < this.Dimation.depth; y++)
            //{
            //    for (int x = 0; x < this.Dimation.width; x++)
            //    {
            //        this.Born(init_matrix[x, y], (x, y));
            //    }
            //}
            foreach(var (x, y) in World.ForEachPos<bool>(init_matrix))
            {
                this.Born(init_matrix[x, y], (x, y));
            }

            _god = new GodPower(this);
        }


        private void Born(bool alive, (int x, int y) position)
        {
            if (this._maps_current_life_sense[position.x, position.y] != null)
            {
                throw new ArgumentOutOfRangeException();
            }

            var cell = new Life(out var sense, alive);
            sense.InitWorldSide(this, position, () =>
            {
                return this.SeeAround(position);
            });
            this._maps_current_life_sense[position.x, position.y] = sense;
        }

        // only God (world) can do this via {GodPower}
        private void TimePass()
        {
            foreach(var (x, y) in World.ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            //for (int y = 0; y < this.Dimation.depth; y++)
            {
                //for (int x = 0; x < this.Dimation.width; x++)
                {
                    this._maps_snapshot[x, y] =
                        this._maps_current_life_sense[x, y].TakeSnapshot();
                }
            }

            foreach (var (x, y) in World.ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            //for (int y = 0; y < this.Dimation.depth; y++)
            {
                //for (int x = 0; x < this.Dimation.width; x++)
                {
                    this._maps_current_life_sense[x, y].TimePass();
                }
            }
        }

        // only God (world) can do this via {GodPower}
        private bool[,] GodVision()
        {
            bool[,] matrix = new bool[this.Dimation.width, this.Dimation.depth];

            foreach (var (x, y) in World.ForEachPos<Life.Sensibility>(this._maps_current_life_sense))
            //for (int y = 0; y < this.Dimation.depth; y++)
            {
                //for (int x = 0; x < this.Dimation.width; x++)
                {
                    matrix[x, y] = (this._maps_current_life_sense[x, y] != null && this._maps_current_life_sense[x, y].Itself.IsAlive);

                }
            }

            return matrix;
        }

        // only life itself can do this
        private Life[,] SeeAround((int x, int y) pos)
        {
            Life[,] result = new Life[3, 3];

            result[0, 0] = this.SeePosition(pos.x - 1, pos.y - 1);
            result[1, 0] = this.SeePosition(pos.x, pos.y - 1);
            result[2, 0] = this.SeePosition(pos.x + 1, pos.y - 1);

            result[0, 1] = this.SeePosition(pos.x - 1, pos.y);
            //result[1, 1] = this.SeePosition(pos.x    , pos.y   );
            result[2, 1] = this.SeePosition(pos.x + 1, pos.y);

            result[0, 2] = this.SeePosition(pos.x - 1, pos.y + 1);
            result[1, 2] = this.SeePosition(pos.x, pos.y + 1);
            result[2, 2] = this.SeePosition(pos.x + 1, pos.y + 1);

            return result;
        }

        private Life SeePosition(int x, int y)
        {
            if (x < 0) return null;
            if (x >= this.Dimation.width) return null;
            if (y < 0) return null;
            if (y >= this.Dimation.depth) return null;
            return this._maps_snapshot[x, y];
        }


        public class GodPower
        {
            private World _reality;

            public GodPower(World reality)
            {
                this._reality = reality;
            }

            public bool[,] SeeWholeWorld()
            {
                return this._reality.GodVision();
            }

            public void TimePass()
            {
                this._reality.TimePass();
            }
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
