using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1
{
    public class World
    {
        public readonly (int width, int depth) Dimation;

        //private WorldContext[,] _matrix;
        private Life[,] _maps_current;
        private Life[,] _maps_snapshot;
        private Dictionary<Life, (int x, int y)> _links = new Dictionary<Life, (int x, int y)>();

        public World(int width, int depth, out GodSensibility _god)
        {
            this.Dimation = (width, depth);
            this._maps_current = new Life[width, depth];
            this._maps_snapshot = new Life[width, depth];

            _god = new GodSensibility(this);
        }

        public World(bool[,] init_matrix, out GodSensibility _god)
        {
            this.Dimation = (init_matrix.GetLength(0), init_matrix.GetLength(1));
            this._maps_current = new Life[this.Dimation.width, this.Dimation.depth];
            this._maps_snapshot = new Life[this.Dimation.width, this.Dimation.depth];

            for (int y = 0; y < this.Dimation.depth; y++)
            {
                for (int x = 0; x < this.Dimation.width; x++)
                {
                    (int x, int y) cell_pos = (x, y);
                    this.Born(
                        new Life(new LifeSensibility(this, cell_pos), init_matrix[cell_pos.x, cell_pos.y]),
                        cell_pos);
                }
            }

            _god = new GodSensibility(this);
        }


        // only God (world) can do this
        private void Born(Life cell, (int x, int y) position)
        {
            if (this._maps_current[position.x, position.y] != null)
            {
                throw new ArgumentOutOfRangeException();
            }

            this._maps_current[position.x, position.y] = cell;
            this._links.Add(cell, position);
        }

        // only God (world) can do this
        private void TimePass()
        {
            for (int y = 0; y < this.Dimation.depth; y++)
            {
                for (int x = 0; x < this.Dimation.width; x++)
                {
                    this._maps_snapshot[x, y] = this._maps_current[x, y].Snapshot;
                }
            }

            for (int y = 0; y < this.Dimation.depth; y++)
            {
                for (int x = 0; x < this.Dimation.width; x++)
                {
                    this._maps_current[x, y].TimePass();
                }
            }
        }

        // only God (world) can do this
        private bool[,] GodVision()
        {
            bool[,] matrix = new bool[this.Dimation.width, this.Dimation.depth];

            for (int y = 0; y < this.Dimation.depth; y++)
            {
                for (int x = 0; x < this.Dimation.width; x++)
                {
                    matrix[x, y] = (this._maps_current[x, y] != null && this._maps_current[x, y].IsAlive);
                }
            }

            return matrix;
        }

        // only life itself can do this
        private Life[,] SeeAround((int x, int y) pos)
        {
            Life[,] result = new Life[3, 3];

            result[0, 0] = this.SeePosition(pos.x - 1, pos.y - 1);
            result[1, 0] = this.SeePosition(pos.x   ,  pos.y - 1);
            result[2, 0] = this.SeePosition(pos.x + 1, pos.y - 1);

            result[0, 1] = this.SeePosition(pos.x - 1, pos.y   );
            //result[1, 1] = this.SeePosition(pos.x    , pos.y   );
            result[2, 1] = this.SeePosition(pos.x + 1, pos.y   );

            result[0, 2] = this.SeePosition(pos.x - 1, pos.y + 1);
            result[1, 2] = this.SeePosition(pos.x    , pos.y + 1);
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


        public class LifeSensibility
        {
            private World _reality;
            private (int x, int y) _position;


            public LifeSensibility(World reality, (int x, int y) pos)
            {
                this._reality = reality;
                this._position = pos;
            }

            public Life[,] SeeAround()
            {
                return this._reality.SeeAround(this._position);
            }
        }

        public class GodSensibility
        {
            private World _reality;

            public GodSensibility(World reality)
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
    }
}
