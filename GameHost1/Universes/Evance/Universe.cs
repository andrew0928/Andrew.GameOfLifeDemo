using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    /// <summary>
    /// 一個單一二維行星的宇宙
    /// </summary>
    public class Universe
    {
        private readonly UniverseSettings _universeSettings;

        private bool _alreadyBigBang = false;
        private Time _time;
        //private Space _space;
        private IPlanet _planet;
        private IEnumerable<ILife> _lives;


        public Universe(UniverseSettings universeSettings)
        {
            _universeSettings = universeSettings;
            BigBang();
        }

        //public Universe(TPlanet planet)
        //{
        //    BigBang(planet);
        //}

        ///// <summary>
        ///// 初始化一個宇宙，有時間但不流逝
        ///// </summary>
        //public Universe()
        //{

        //}

        ///// <summary>
        ///// 初始化一個宇宙，有時間、空間、生命(物質)
        ///// </summary>
        ///// <param name="time"></param>
        ///// <param name="planet"></param>
        ///// <param name="lives"></param>
        //public Universe(Time time, TPlanet planet, ILife[,] lives)
        //{

        //}

        public void MakeTimeElapseOnce()
        {
            this._time?.ElapseOnce();
        }


        //public Universe(bool[,] matrix, UniverseSettings universeSettings)
        //{
        //    _universeSettings = universeSettings ?? new UniverseSettings();


        //    BigBang(planet);
        //}

        private void BigBang()
        {
            if (_alreadyBigBang)
            {
                return;
            }

            _alreadyBigBang = true;

            _time = new Time(_universeSettings.TimeSettings);

            _planet = new Planet(_universeSettings.MaxCoordinatesX, _universeSettings.MaxCoordinatesY);

            _lives = GenerateLives();

            if (_universeSettings.EnableAutoMode)
            {
                _time.Ready += (sender, eventArgs) => this.ShowViewOfPlanet(sender, eventArgs);
                _time.Start();
            }
        }

        protected IEnumerable<Life> GenerateLives()
        {
            var lives = new List<Life>();

            for (int x = 0; x < _universeSettings.MaxCoordinatesX; x++)
            {
                for (int y = 0; y < _universeSettings.MaxCoordinatesY; y++)
                {
                    var life = new Life(
                        _time,
                        _planet,
                        new LifeSettings()
                        {
                            Coordinates = new int[] { x, y },
                            IsAlive = _universeSettings.DefaultAliveLivesMatrix[x, y]
                        });

                    _planet.TryPutLife(life);

                    //yield return life;
                    lives.Add(life);
                }
            }

            return lives;
        }

        public void GoToNextTime()
        {
            _time.ElapseOnce();
        }



        ///// <summary>
        ///// 宇宙大爆炸。
        ///// 不同於現實中推論的宇宙大爆炸，這個宇宙以一顆二維星球(二維的球!?)取代空間的概念，並且立刻演化出生命。
        ///// </summary>
        //private void BigBang(TPlanet planet)
        //{
        //    if (_alreadyBigBang)
        //    {
        //        return;
        //    }

        //    _alreadyBigBang = true;

        //    _planet = planet;

        //    _time = new Time(_universeSettings.TimeSettings, _universeSettings.AutoStartTime);
        //    _time.Elapsing += (sender, eventArgs) => _planet.TimeElapsed(sender, eventArgs);
        //    _time.Elapsing += (sender, eventArgs) => this.ShowViewOfPlanet(sender, eventArgs);
        //    _time.Start();
        //}

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
