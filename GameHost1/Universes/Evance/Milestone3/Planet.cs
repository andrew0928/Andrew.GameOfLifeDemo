using System;
using System.Collections.Generic;

namespace GameHost1.Universes.Evance.Milestone3
{
    public class Planet : IPlanet
    {
        private readonly Life[,] _livesMatrix;

        public (int XAsis, int YAsis) MaxCoordinates { get; }

        /// <summary>
        /// 上一個週期所有 Lives 的外觀，僅能查看 preperties 。
        /// </summary>
        public LifeWithAppearanceOnly[,] LastFrameLives { get; private set; }

        public Planet((int XAsis, int YAsis) maxCoordinates)
        {
            // TODO: 檢查

            MaxCoordinates = maxCoordinates;

            _livesMatrix = new Life[this.MaxCoordinates.XAsis, this.MaxCoordinates.YAsis];
            LastFrameLives = new LifeWithAppearanceOnly[this.MaxCoordinates.XAsis, this.MaxCoordinates.YAsis];
        }

        public virtual bool TryPutLife(ILife iLife)
        {
            var life = iLife as Life;

            if (life?.Coordinates == null ||
                life.Coordinates.X < 0 || life.Coordinates.X >= this.MaxCoordinates.XAsis ||
                life.Coordinates.Y < 0 || life.Coordinates.Y >= this.MaxCoordinates.YAsis)
            {
                throw new ArgumentOutOfRangeException(nameof(Life.Coordinates));
            }

            if (_livesMatrix[life.Coordinates.X, life.Coordinates.Y] == null)
            {
                _livesMatrix[life.Coordinates.X, life.Coordinates.Y] = life;

                return true;
            }

            return false;
        }

        public virtual void RefreshLastFrameLives()
        {
            foreach (var c in this.MaxCoordinates.ForEach())
            {
                LastFrameLives[c.X, c.Y] = _livesMatrix[c.X, c.Y].ConvertToAppearanceOnlyLife();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinatesOfTartgets"></param>
        /// <returns></returns>
        public virtual int GetAliveLivesCount(IEnumerable<(int X, int Y)> coordinatesOfTartgets)
        {
            int aliveLivesCount = 0;
            foreach (var targetCoordinates in coordinatesOfTartgets)
            {
                aliveLivesCount += (LastFrameLives[targetCoordinates.X, targetCoordinates.Y]?.IsAlive ?? false) ? 1 : 0;
            }

            return aliveLivesCount;
        }

        public int GetAroundAliveLivesCount((int X, int Y) tartgetCoordinates)
        {
            var nearbyCoornates = GetCoordinatesAroundTartget(tartgetCoordinates);

            return GetAliveLivesCount(nearbyCoornates);
        }

        private List<(int X, int Y)> GetCoordinatesAroundTartget((int X, int Y) tartgetCoordinates)
        {
            int areaX = 3;
            int areaY = 3;

            var nearbyCoornates = new List<(int X, int Y)>(areaX * areaY);

            foreach (var p in (areaX, areaY).ForEach())
            {
                var xCoordinate = tartgetCoordinates.X - 1 + p.X;
                var yCoordinate = tartgetCoordinates.Y - 1 + p.Y;

                if (xCoordinate >= 0 && xCoordinate < MaxCoordinates.XAsis &&
                    yCoordinate >= 0 && yCoordinate < MaxCoordinates.YAsis)
                {
                    nearbyCoornates.Add((xCoordinate, yCoordinate));
                }
            }

            nearbyCoornates.RemoveAll(c => c.X == tartgetCoordinates.X && c.Y == tartgetCoordinates.Y);

            return nearbyCoornates;
        }
    }
}
