using System;
using System.Collections.Generic;

namespace GameHost1.Universes.Evance
{
    /// <summary>
    /// A 2D planet base class.
    /// </summary>
    public abstract class PlanetBase : IPlanet
    {
        private readonly int _x;
        private readonly int _y;
        private readonly ILife[,] _livesMatrix;

        public int[] MaxCoordinates { get; }

        public PlanetBase(int x, int y)
        {
            _x = x > 0 ? x : throw new ArgumentOutOfRangeException(nameof(x));
            _y = y > 0 ? y : throw new ArgumentOutOfRangeException(nameof(y));

            _livesMatrix = new ILife[x, y];

            MaxCoordinates = new int[] { _x, _y };
        }

        public PlanetBase(ILife[,] lives)
        {
            if (lives == null)
            {
                throw new ArgumentNullException(nameof(lives));
            }

            _livesMatrix = lives;
            _x = _livesMatrix.GetLength(0);
            _y = _livesMatrix.GetLength(1);

            MaxCoordinates = new int[] { _x, _y };
        }

        public virtual bool TryPutLife(ILife life)
        {
            if (life?.Coordinates == null || life.Coordinates.Length != 2 ||
                life.Coordinates[0] < 0 || life.Coordinates[0] > this._x ||
                life.Coordinates[1] < 0 || life.Coordinates[1] > this._y)
            {
                throw new ArgumentOutOfRangeException(nameof(life.Coordinates));
            }

            if (_livesMatrix[life.Coordinates[0], life.Coordinates[1]] == null)
            {
                _livesMatrix[life.Coordinates[0], life.Coordinates[1]] = life;

                return true;
            }

            return false;
        }

        public virtual bool[,] ShowLivesAreAlive()
        {
            var areLivesAlive = new bool[_x, _y];

            for (int x = 0; x < _x; x++)
            {
                for (int y = 0; y < _y; y++)
                {
                    areLivesAlive[x, y] = _livesMatrix[x, y]?.IsAlive ?? false;
                }
            }

            return areLivesAlive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinatesOfTartgets"></param>
        /// <returns></returns>
        public virtual int GetAliveLivesCount(IEnumerable<int[]> coordinatesOfTartgets)
        {
            int aliveLivesCount = 0;
            foreach (var targetCoordinates in coordinatesOfTartgets)
            {
                if (targetCoordinates.Length == 2)
                {
                    aliveLivesCount += (_livesMatrix[targetCoordinates[0], targetCoordinates[1]]?.IsAlive ?? false) ? 1 : 0;
                }
            }

            return aliveLivesCount;
        }
    }
}
