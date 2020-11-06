using System;
using System.Collections.Generic;
using System.Threading;

namespace GameHost1.Universes.Evance.Milestone3
{
    /// <summary>
    /// 宇宙，有時間(Time)、空間(IPlanet)、物質(IEnumerable<ILife>)
    /// </summary>
    public class Universe : IWorld
    {
        private bool _alreadyInitialize = false;
        private bool _alreadyBigBang = false;
        private Time _time;
        private Planet _planet;
        private SemaphoreSlim _evolvedSignal;

        public (int width, int depth) Dimation { get; }

        public Universe(int width, int depth, int maxConcurrentEvolvingCount = 2)
        {
            this.Dimation = (width, depth);

            _evolvedSignal = new SemaphoreSlim(maxConcurrentEvolvingCount, maxConcurrentEvolvingCount);
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

            // TODO: 每一次 run 都去找有哪些 life 需要演化，把 life 的進化當成排程任務。

            // 先把時間演進塞給所有的 lives ，可以得到這個 round 會演化的 lives

            do
            {
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

            _time = new Time(new TimeSettings()
            {
                StartDelay = 0,
                Interval = world_frame,
            });

            _planet = new Planet(this.Dimation);

            GenerateLives(init_matrix, init_cell_frame, init_cell_start_frame);

            // 建立第一份生命快照
            _planet.RefreshLastFrameLives();

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
                    EvolvedSignal = _evolvedSignal,
                };

                var life = new Life(lifeSettings);

                _planet.TryPutLife(life);
            }

            return lives;
        }
    }
}
