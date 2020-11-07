using System;
using System.Collections.Generic;

namespace GameHost1.Universes.Evance.Milestone3
{
    /// <summary>
    /// 宇宙，有時間(Time)、空間(Planet)，空間上有物質(Life)
    /// </summary>
    public class Universe : IWorld
    {
        private bool _alreadyInitialize = false;
        private bool _alreadyBigBang = false;
        private Time _time;
        private Planet _planet;

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

            #region 檢查參數

            CheckInputMatrixDimation(init_matrix);
            CheckInputMatrixDimation(init_cell_frame);
            CheckInputMatrixDimation(init_cell_start_frame);

            #endregion

            BigBang(init_matrix, init_cell_frame, init_cell_start_frame, world_frame);

            _alreadyInitialize = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="until"></param>
        /// <param name="realtime"></param>
        /// <returns></returns>
        public IEnumerable<(TimeSpan time, ILife[,] matrix)> Running(TimeSpan until, bool realtime = false)
        {
            while (until > _time.CurrentTime)
            {
                _time.ElapseOnce();

                _planet.RefreshLastFrameLives();

                yield return (_time.CurrentTime, _planet.LastFrameLives);
            }
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
            foreach (var p in ArrayHelper.ForEachPos(init_matrix))
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

                _planet.TryPutLife(life);
            }

            return lives;
        }

        private void CheckInputMatrixDimation<T>(T[,] inputMatrix)
        {
            if (inputMatrix.Rank != 2 ||
                inputMatrix.GetLength(0) != this.Dimation.width ||
                inputMatrix.GetLength(1) != this.Dimation.depth)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
