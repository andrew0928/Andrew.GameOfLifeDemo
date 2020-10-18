using System;
using System.Collections.Generic;

namespace GameHost1.Universes.Evance
{
    /// <summary>
    /// 宇宙，有時間(Time)、空間(IPlanet)、物質(IEnumerable<ILife>)
    /// </summary>
    public class Universe
    {
        private readonly UniverseSettings _universeSettings;

        private bool _alreadyBigBang = false;
        private ITime _time;
        private IPlanet _planet;
        private ILifeFactory _lifeFactory;
        private IEnumerable<ILife> _lives;

        public Universe(UniverseSettings universeSettings)
        {
            _universeSettings = universeSettings;

            BigBang();
        }

        public void MakeTimeElapseOnce()
        {
            this._time.ElapseOnce();
        }

        private void BigBang()
        {
            if (_alreadyBigBang)
            {
                return;
            }

            _alreadyBigBang = true;

            _time = new Time(_universeSettings.TimeSettings);

            _planet = new Planet(_universeSettings.MaxCoordinatesX, _universeSettings.MaxCoordinatesY);

            _lifeFactory = _universeSettings?.LifeFactory ?? new LifeFactory();

            _lives = GenerateLives();

            if (_universeSettings.EnableAutoMode)
            {
                _time.Ready += (sender, eventArgs) => this.ShowViewOfPlanet(sender, eventArgs);
                _time.Start();
            }
        }

        private IEnumerable<ILife> GenerateLives()
        {
            var lives = new List<ILife>();

            for (int x = 0; x < _universeSettings.MaxCoordinatesX; x++)
            {
                for (int y = 0; y < _universeSettings.MaxCoordinatesY; y++)
                {
                    var lifeSettings = new LifeSettings()
                    {
                        Time = _time,
                        Planet = _planet,
                        Coordinates = new int[] { x, y },
                        IsAlive = _universeSettings.DefaultAliveLivesMatrix[x, y]
                    };

                    var life = _lifeFactory.Generate(lifeSettings);

                    _planet.TryPutLife(life);

                    //yield return life;
                    lives.Add(life);
                }
            }

            return lives;
        }

        public bool[,] ShowLivesAreAlive()
        {
            var aliveLivesMatrix = new bool[_universeSettings.MaxCoordinatesX, _universeSettings.MaxCoordinatesY];

            foreach (var life in _lives)
            {
                aliveLivesMatrix[life.Coordinates[0], life.Coordinates[1]] = life.IsAlive;
            }

            return aliveLivesMatrix;
        }

        public void ShowViewOfPlanet(int round)
        {
            Console.SetCursorPosition(0, 0);
            int live_count = 0;

            var aliveLivesMatrix = ShowLivesAreAlive();
            for (int y = 0; y < aliveLivesMatrix.GetLength(1); y++)
            {
                for (int x = 0; x < aliveLivesMatrix.GetLength(0); x++)
                {
                    var c = aliveLivesMatrix[x, y];
                    live_count += (c ? 1 : 0);
                    Console.Write(c ? '★' : '☆');
                }
                Console.WriteLine();
            }

            Console.WriteLine($"total lives: {live_count}, round: {round} / {_universeSettings.TimeSettings.MaxGeneration}...");
        }

        public void ShowViewOfPlanet(object sender, TimeEventArgs e)
        {
            ShowViewOfPlanet(e?.CurrentGeneration ?? 0);
        }
    }
}
