using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance.Milestone3
{
    public class World : IWorld
    {
        private bool _alreadyInitialize = false;

        /// <summary>
        /// msec
        /// </summary>
        private int _intervalFrame;

        /// <summary>
        /// msec
        /// </summary>
        private int _currentFrame = 0;

        private TimeSpan _currentTime = new TimeSpan();

        private Life[,] _lives;

        /// <summary>
        /// 上一個週期所有 Lives 的外觀，僅能查看 preperties 。
        /// </summary>
        public LifeWithAppearanceOnly[,] LastFrameLives { get; private set; }

        public readonly (int width, int depth) Dimation;

        public World(int width, int depth)
        {
            this.Dimation = (width, depth);

            _lives = new Life[this.Dimation.width, this.Dimation.depth];
            LastFrameLives = new LifeWithAppearanceOnly[this.Dimation.width, this.Dimation.depth];
        }

        public bool Init(bool[,] init_matrix, int[,] init_cell_frame, int[,] init_cell_start_frame, int world_frame)
        {
            // 沒考慮多執行緒 Init 的情境
            if (_alreadyInitialize)
            {
                return false;
            }

            // TODO: 檢查參數

            //// 初始化所有的生命
            //foreach (var p in World.ForEachPos(init_matrix))
            //{
            //    var life = new Life();
            //    life.Init(init_matrix[p.x, p.y], init_cell_frame[p.x, p.y], init_cell_start_frame[p.x, p.y]);

            //    // TODO: 檢查 life.Init 是否成功

            //    _lives[p.x, p.y] = life;

            //    // 建立第一份生命快照
            //    LastFrameLives[p.x, p.y] = life.ConvertToAppearanceOnlyLife();
            //}

            //_intervalFrame = world_frame;

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
                _currentTime.Add(TimeSpan.FromMilliseconds(_intervalFrame));





                yield return (_currentTime, LastFrameLives);
            } while (until > _currentTime);
        }


        // TODO: 要做一個讓 life 查找附近其他 lives 的方法，但要確保 life 查找時只能用自身的條件去找。

        // utility, 簡化到處都出現的雙層迴圈。只會循序取出 2D 陣列中所有的 (x, y) 座標組合
        public static IEnumerable<(int x, int y)> ForEachPos<T>(T[,] array)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                for (int x = 0; x < array.GetLength(0); x++)
                {
                    yield return (x, y);
                }
            }
        }
    }
}
