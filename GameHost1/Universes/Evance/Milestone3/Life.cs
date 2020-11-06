using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance.Milestone3
{
    //public class Milestone3Life : ILife
    //{
    //    private bool _areadyInit = false;
    //    private bool _isAlive = false;
    //    private bool _appearanceOnly = false;
    //    private int _generation = 0;
    //    private int _intervalFrame;
    //    private int _startAfterFrame;
    //    /// <summary>
    //    /// 還差多少 msec 就可以進化
    //    /// </summary>
    //    private int _leftTimeForEvolving;

    //    public bool IsAlive => _isAlive;

    //    public bool AppearanceOnly => _appearanceOnly;

    //    public int Generation => _generation;

    //    public Milestone3Life()
    //    {
    //    }

    //    /// <summary>
    //    /// 建構花瓶 Life ，只能訪問 properties 。
    //    /// </summary>
    //    /// <param name="isAlive"></param>
    //    public Milestone3Life(bool isAlive, int generation)
    //    {
    //        _appearanceOnly = true;
    //        _isAlive = isAlive;
    //        _generation = generation;
    //    }

    //    public bool Init(bool isAlive, int intervalFrame, int startAfterFrame)
    //    {
    //        if (_areadyInit || _appearanceOnly)
    //        {
    //            return false;
    //        }

    //        _areadyInit = true;

    //        _isAlive = isAlive;
    //        _intervalFrame = intervalFrame;
    //        _startAfterFrame = startAfterFrame;

    //        _leftTimeForEvolving = _startAfterFrame;

    //        return true;
    //    }


    //    public bool IsGoingOnToEvolve(TimeSpan time)
    //    {
    //        return time.TotalMilliseconds >= _leftTimeForEvolving;
    //    }

    //    public void TimePass()
    //    {
    //        // 環境一次週期期間， life 刷新一次跟多次是相同意義



    //    }

    //    public void Evolve()
    //    {

    //    }


    //    public LifeWithAppearanceOnly ConvertToAppearanceOnlyLife()
    //    {
    //        return new LifeWithAppearanceOnly(this);
    //    }

    //    /// <summary>
    //    /// 取得花瓶 Life 。
    //    /// </summary>
    //    /// <param name="life"></param>
    //    /// <returns></returns>
    //    public static LifeWithAppearanceOnly GetAppearanceOnlyLife(Life life)
    //    {
    //        return new LifeWithAppearanceOnly(life);
    //    }
    //}

    public class Life : ILife, IDisposable
    {
        private readonly ITimeReadOnly _time;
        private readonly IPlanetReadOnly _planet;
        private readonly LifeSettings _lifeSettings;
        /// <summary>
        /// 下一次有效進化時間 (在同一個時間區間內，進化一次跟進化多次的意義相同)
        /// </summary>
        private TimeSpan _leftTimeForEvolving;
        private readonly TimeSpan _intervalTimespan;
        private bool isDisposed;

        public bool IsAlive { get; private set; }

        public (int X, int Y) Coordinates { get; }

        public Life(LifeSettings lifeSettings)
        {
            #region 檢查傳入參數

            _time = lifeSettings?.Time ?? throw new ArgumentNullException(nameof(LifeSettings.Time));

            _planet = lifeSettings?.Planet ?? throw new ArgumentNullException(nameof(LifeSettings.Planet));

            _lifeSettings = lifeSettings ?? throw new ArgumentNullException(nameof(lifeSettings));

            CheckSettings();

            #endregion

            IsAlive = _lifeSettings.InitIsAlive;

            Coordinates = _lifeSettings.InitCoordinates;

            // 初始化下一次演化的時間，即為第一次演化的時間
            _leftTimeForEvolving = TimeSpan.FromMilliseconds(_lifeSettings.TimeSettings.StartDelay);

            _intervalTimespan = TimeSpan.FromMilliseconds(_lifeSettings.TimeSettings.Interval);

            _time.Elapsing += (sender, eventArgs) => this.TryEvolve(sender, eventArgs);
        }

        private void CheckSettings()
        {
            // TODO

            //if (_lifeSettings.EvolutionInterval < 0)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(_lifeSettings.EvolutionInterval), "must bigger than 0");
            //}
        }

        protected virtual bool CanAdaptToEnvironment(int aroundAliveLivesCount)
        {
            if (this.IsAlive)
            {
                if (aroundAliveLivesCount < 2 || aroundAliveLivesCount > 3)
                {
                    return false;
                }
            }
            else
            {
                if (aroundAliveLivesCount == 3)
                {
                    return true;
                }
            }

            return this.IsAlive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="timeEventArgs"></param>
        /// <returns>是否有演化</returns>
        protected virtual bool TryEvolve(object sender, TimeEventArgs timeEventArgs)
        {
            // 先看看演化時間到了沒
            if (timeEventArgs.CurrentTime >= this._leftTimeForEvolving)
            {
                // 演化
                var aroundAliveLivesCount = _planet.GetAroundAliveLivesCount(this.Coordinates);

                this.IsAlive = CanAdaptToEnvironment(aroundAliveLivesCount);

                // 計算下一次有效演化時間
                do
                {
                    this._leftTimeForEvolving = this._leftTimeForEvolving.Add(_intervalTimespan);
                } while (this._leftTimeForEvolving >= timeEventArgs.NextTime);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 轉換為花瓶 Life 。
        /// </summary>
        /// <returns></returns>
        public LifeWithAppearanceOnly ConvertToAppearanceOnlyLife()
        {
            return new LifeWithAppearanceOnly(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    _time.Elapsing -= (sender, eventArgs) => this.TryEvolve(sender, eventArgs);
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
