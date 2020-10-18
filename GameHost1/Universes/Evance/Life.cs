using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public class Life : ILife
    {
        private readonly Time _time;
        private readonly IPlanet _planet;
        private readonly LifeSettings _lifeSettings;
        private bool _isAlive;
        private bool _isAliveAtNextGeneration;
        private IEnumerable<Func<bool>> _survivalConditions;

        public int[] Coordinates { get; private set; }

        public IEnumerable<int[]> CoordinateOfNearbyLives { get; private set; }

        public bool IsAlive => _isAlive;


        public Life(Time time, IPlanet planet, LifeSettings lifeSettings)
        {
            _time = time ?? throw new ArgumentNullException(nameof(time));
            _time.Elapsing += (sender, eventArgs) => this.AdaptToEnvironment(sender, eventArgs);
            _time.Elapsed += (sender, eventArgs) => this.Evolve();

            _planet = planet ?? throw new ArgumentNullException(nameof(planet));

            _lifeSettings = lifeSettings ?? throw new ArgumentNullException(nameof(lifeSettings));
            CheckSettings();

            Coordinates = _lifeSettings.Coordinates;

            _isAlive = _lifeSettings.IsAlive;

            CoordinateOfNearbyLives = InitializeCoordinateOfNearbyLives();

            _survivalConditions = InitializeSurvivalConditions();

            //// 自身狀態
            //// 感知周遭
            //// 演化結果

            ////var cell = new Cell(area);
            ////cell.Evolve();

            ////return cell.IsLive;

            //return new Cell(area).Evolve();
        }


        private void CheckSettings()
        {
            if (_lifeSettings.Coordinates == null || _lifeSettings.Coordinates.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(_lifeSettings.Coordinates), "must be int[2]");
            }

            if (_lifeSettings.EvolutionInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_lifeSettings.EvolutionInterval), "must bigger than 0");
            }


        }

        //public Life(Time time, IPlanet planet, int[] coordinate, bool isAlive)
        //{
        //    _time = time ?? throw new ArgumentNullException(nameof(time));
        //    _time.Elapsing += (sender, eventArgs) => this.AdaptToEnvironment();
        //    _time.Elapsed += (sender, eventArgs) => this.Evolve();

        //    _planet = planet ?? throw new ArgumentNullException(nameof(planet));

        //    if (coordinate == null || coordinate.Length != 2)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(coordinate), "must be int[2]");
        //    }

        //    Coordinates = coordinate;

        //    _isAlive = isAlive;

        //    CoordinateOfNearbyLives = InitializeCoordinateOfNearbyLives();

        //    _survivalConditions = InitializeSurvivalConditions();

        //    //// 自身狀態
        //    //// 感知周遭
        //    //// 演化結果

        //    ////var cell = new Cell(area);
        //    ////cell.Evolve();

        //    ////return cell.IsLive;

        //    //return new Cell(area).Evolve();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns>int[][2]</returns>
        protected virtual IEnumerable<int[]> InitializeCoordinateOfNearbyLives()
        {
            int areaX = 3;
            int areaY = 3;

            var nearbyCoornates = new List<int[]>();

            for (int x = 0; x < areaX; x++)
            {
                for (int y = 0; y < areaY; y++)
                {
                    var xCoordinate = this.Coordinates[0] - 1 + x;
                    var yCoordinate = this.Coordinates[1] - 1 + y;

                    if (xCoordinate >= 0 && xCoordinate < _planet.MaxCoordinates[0] &&
                        yCoordinate >= 0 && yCoordinate < _planet.MaxCoordinates[1])
                    {
                        //yield return new int[] { xCoordinate, yCoordinate };


                        nearbyCoornates.Add(new int[] { xCoordinate, yCoordinate });
                    }
                }
            }

            nearbyCoornates.RemoveAll(c => c[0] == this.Coordinates[0] && c[1] == this.Coordinates[1]);

            return nearbyCoornates;
        }

        protected virtual IEnumerable<Func<bool>> InitializeSurvivalConditions()
        {
            var conditions = new List<Func<bool>>();


            return conditions;
        }

        protected virtual bool CanAdaptToEnvironment(int currentGeneration)
        {
            if (currentGeneration % _lifeSettings.EvolutionInterval != 0)
            {
                return this.IsAlive;
            }

            var nearbyAliveLivesCount = _planet.GetAliveLivesCount(this.CoordinateOfNearbyLives);

            if (this.IsAlive)
            {
                if (nearbyAliveLivesCount < 2 || nearbyAliveLivesCount > 3)
                {
                    return false;
                }
            }
            else
            {
                if (nearbyAliveLivesCount == 3)
                {
                    return true;
                }
            }

            return this.IsAlive;
        }

        public virtual void AdaptToEnvironment(object sender, TimeEventArgs timeEventArgs)
        {
            //Console.WriteLine($"AdaptToEnvironment - life coordinates: [{this.Coordinates[0]}, {this.Coordinates[1]}]");

            _isAliveAtNextGeneration = CanAdaptToEnvironment(timeEventArgs.CurrentGeneration);
        }

        public virtual bool Evolve()
        {
            //Console.WriteLine($"Evolve - life coordinates: [{this.Coordinates[0]}, {this.Coordinates[1]}]");

            this._isAlive = this._isAliveAtNextGeneration;

            return this._isAlive;
        }
    }
}
