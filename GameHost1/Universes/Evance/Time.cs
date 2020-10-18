using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameHost1.Universes.Evance
{
    public class Time : ITime, IDisposable
    {
        private TimeSettings _timeSettings;

        private int _currentGeneration;
        private Task _elaspeTask;
        private volatile bool _elapseSignal = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool isDisposed;

        public event EventHandler<TimeEventArgs> Ready;
        public event EventHandler<TimeEventArgs> Elapsing;
        public event EventHandler<TimeEventArgs> Elapsed;

        public int CurrentGeneration => _currentGeneration;

        public Time() : this(new TimeSettings())
        {
        }

        public Time(TimeSettings timeSettings)
        {
            _timeSettings = timeSettings;

            this.AutoElapse();
        }

        public void Start()
        {
            CheckSettings();

            _elapseSignal = true;
        }

        public void Stop()
        {
            CheckSettings();

            _elapseSignal = false;
        }

        public void ElapseOnce()
        {
            var generation = Interlocked.Increment(ref _currentGeneration);

            var timeEventArgs = new TimeEventArgs()
            {
                CurrentGeneration = generation
            };

            OnReady(timeEventArgs);

            OnElapsing(timeEventArgs);

            OnElapsed(timeEventArgs);
        }

        protected virtual void OnReady(TimeEventArgs e)
        {
            Ready?.Invoke(this, e);
        }

        protected virtual void OnElapsing(TimeEventArgs e)
        {
            Elapsing?.Invoke(this, e);
        }

        protected virtual void OnElapsed(TimeEventArgs e)
        {
            Elapsed?.Invoke(this, e);
        }

        private void AutoElapse()
        {
            CheckSettings();

            _elaspeTask = Task.Run(() =>
            {
                for (int currentGeneration = 0; currentGeneration < _timeSettings.MaxGeneration; currentGeneration++)
                {
                    SpinWait.SpinUntil(() => _elapseSignal);

                    this.ElapseOnce();

                    Thread.Sleep(_timeSettings.PlanckTimeInMillisecond);
                }
            },
            _cancellationTokenSource.Token);
        }

        private void CheckSettings()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(Time));
            }

            if (_timeSettings == null)
            {
                throw new ArgumentNullException(nameof(TimeSettings));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    _cancellationTokenSource.Cancel();
                    _elaspeTask?.Dispose();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                isDisposed = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~Time()
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
