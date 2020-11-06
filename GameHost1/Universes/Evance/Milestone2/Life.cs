using System;
using System.Collections.Generic;

namespace GameHost1.Universes.Evance.Milestone2
{
    public class Life : ILife, IDisposable
    {
        private readonly ITimeReadOnly _time;
        private readonly IPlanetReadOnly _planet;
        private readonly LifeSettings _lifeSettings;
        private bool _isAliveAtNextGeneration;
        private bool isDisposed;

        public int[] Coordinates { get; private set; }

        public IEnumerable<int[]> CoordinateOfNearbyLives { get; private set; }

        public bool IsAlive { get; private set; }

        public Life(LifeSettings lifeSettings)
        {
            _time = lifeSettings?.Time ?? throw new ArgumentNullException(nameof(LifeSettings.Time));
            _time.Elapsing += (sender, eventArgs) => this.AdaptToEnvironment(sender, eventArgs);
            _time.Elapsed += (sender, eventArgs) => this.Evolve();

            _planet = lifeSettings?.Planet ?? throw new ArgumentNullException(nameof(LifeSettings.Planet));

            _lifeSettings = lifeSettings ?? throw new ArgumentNullException(nameof(lifeSettings));
            CheckSettings();

            Coordinates = _lifeSettings.Coordinates;

            IsAlive = _lifeSettings.IsAlive;

            CoordinateOfNearbyLives = InitializeCoordinateOfNearbyLives();
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
            _isAliveAtNextGeneration = CanAdaptToEnvironment(timeEventArgs.CurrentGeneration);
        }

        public virtual bool Evolve()
        {
            this.IsAlive = this._isAliveAtNextGeneration;

            return this.IsAlive;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)

                    _time.Elapsing -= (sender, eventArgs) => this.AdaptToEnvironment(sender, eventArgs);
                    _time.Elapsed -= (sender, eventArgs) => this.Evolve();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                isDisposed = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~Life()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
