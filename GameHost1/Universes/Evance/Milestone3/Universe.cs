using System;
using System.Collections.Generic;

namespace GameHost1.Universes.Evance.Milestone3
{
    /// <summary>
    /// 宇宙，有時間(Time)、空間(IPlanet)、物質(IEnumerable<ILife>)
    /// </summary>
    public class Universe : IWorld
    {
        private bool _alreadyBigBang = false;
        private Time _time;
        private Planet _planet;




        private bool _alreadyInitialize = false;

        ///// <summary>
        ///// msec
        ///// </summary>
        //private int _intervalFrame;

        ///// <summary>
        ///// msec
        ///// </summary>
        //private int _currentFrame = 0;

        //private TimeSpan _currentTime = new TimeSpan();

        public List<Life> Lives { get; private set; } = new List<Life>();

        public (int width, int depth) Dimation { get; }

        public Universe(int width, int depth)
        {
            this.Dimation = (width, depth);
        }

        /// <summary>
        /// Initialize world.
        /// </summary>
        /// <param name="init_matrix">lives 初始狀態，是否活著</param>
        /// <param name="init_cell_frame">lives 各自的活動週期 (msec)</param>
        /// <param name="init_cell_start_frame">lives 第一次活動的延遲時間 (msec)</param>
        /// <param name="world_frame">世界的刷新週期</param>
        /// <returns></returns>
        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            // 沒考慮多執行緒 Init 的情境
            if (_alreadyInitialize)
            {
                return false;
            }

            // TODO: 檢查參數

            BigBang(init_matrix, init_cell_frame, init_cell_start_frame, world_frame);

            _alreadyInitialize = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="until"></param>
        /// <returns></returns>
        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until)
        {
            // TODO: 檢查是芥末日到了沒



            //外界環境 (matrix) 會以固定的週期 (預設: 10 msec ) 刷新。在每個周期內的細胞，只能感知到上個週期身邊細胞的狀態。
            //例如，在 300ms ~ 310ms 時段內，A 細胞看到她周圍細胞狀態，會是 290ms ~ 300ms 期間內的結果。A 細胞在他自身週期到的時候，改變的狀態，其他細胞要在下個週期 ( 310ms ~ 320ms ) 才能感知的到。

            //如果細胞跟環境的刷新時間剛好都重疊再一起，則變化順序以細胞的演進為優先，這瞬間演進的結果會出現在這次的環境刷新內。


            // TODO: 每一次 run 都去找有哪些 life 需要演化，把 life 的進化當成排程任務。

            // 先把時間演進塞給所有的 lives ，可以得到這個 round 會演化的 lives

            do
            {
                //_currentTime.Add(TimeSpan.FromMilliseconds(_intervalFrame));

                _time.ElapseOnce();





                _planet.RefreshLastFrameLives();

                yield return (_time.CurrentTime, _planet.LastFrameLives);
            } while (until > _time.CurrentTime);
        }



        private bool BigBang(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            if (_alreadyBigBang)
            {
                return false;
            }

            //_time = new Time(_universeSettings.TimeSettings);
            _time = new Time(new TimeSettings()
            {
                StartDelay = 0,
                Interval = world_frame,
            });

            _planet = new Planet(this.Dimation);

            //_lifeFactory = _universeSettings?.LifeFactory ?? new LifeFactory();

            Lives = GenerateLives(init_matrix, init_cell_frame, init_cell_start_frame);

            // 建立第一份生命快照
            _planet.RefreshLastFrameLives();

            //if (_universeSettings.EnableAutoMode)
            //{
            //    _time.Ready += (sender, eventArgs) => this.ShowViewOfPlanet(sender, eventArgs);
            //    _time.Start();
            //}

            _alreadyBigBang = true;

            return true;
        }


        private List<Life> GenerateLives(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame)
        {
            var lives = new List<Life>();

            // 初始化所有的生命
            foreach (var p in World.ForEachPos(init_matrix))
            {
                var lifeSettings = new LifeSettings()
                {
                    Time = this._time,
                    Planet = this._planet,
                    InitCoordinates = p,
                    InitIsAlive = init_matrix[p.x, p.y],
                    TimeSettings = new TimeSettings()
                    {
                        StartDelay = init_cell_start_frame[p.x, p.y],
                        Interval = init_cell_frame[p.x, p.y],
                    },
                };


                var life = new Life(lifeSettings);
                //life.Init(init_matrix[p.x, p.y], init_cell_frame[p.x, p.y], init_cell_start_frame[p.x, p.y]);

                // TODO: 檢查 life.Init 是否成功

                //_lives[p.x, p.y] = life;
                Lives.Add(life);

                _planet.TryPutLife(life);

                //LastFrameLives[p.x, p.y] = life.ConvertToAppearanceOnlyLife();
            }

            return lives;
        }









    }
}
