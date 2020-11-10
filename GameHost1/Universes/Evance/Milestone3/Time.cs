using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GameHost1.Universes.Evance.Milestone3
{
    //public class Time : ITime, IDisposable
    public class Time : IDisposable
    {
        private TimeSettings _timeSettings;
        private Task _elaspeTask;
        private volatile bool _elapseSignal = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool isDisposed;

        //public event EventHandler<TimeEventArgs> Ready;
        //public event EventHandler<TimeEventArgs> Elapsing;
        //public event EventHandler<TimeEventArgs> Elapsing1;
        //public event EventHandler<TimeEventArgs> Elapsed;


        private int _elapsingEventCount = 0;

        private BufferBlock<TimeEventArgs> _bufferBlock;
        private BroadcastBlock<TimeEventArgs> _broadcaster;
        private List<ActionBlock<TimeEventArgs>> _actionBlocks = new List<ActionBlock<TimeEventArgs>>();
        private CountdownEvent _countdownEvent;

        public int EventHandlersCount { get; private set; }
        public ConcurrentDictionary<int, TimeElapsingEvent> TimeElapsingEventHandlers { get; private set; } = new ConcurrentDictionary<int, TimeElapsingEvent>();

        //private bool TryEvolve(object sender, TimeEventArgs timeEventArgs)
        //{
        //    return true;
        //}

        //public void AddElapsingEvent(Func<object, TimeEventArgs, bool> func)
        //{
        //    var currentelapsingEventCount = Interlocked.Increment(ref _elapsingEventCount);

        //    if (currentelapsingEventCount % 2 == 0)
        //    {
        //        Elapsing += (sender, eventArgs) => func(sender, eventArgs);
        //    }
        //    else
        //    {
        //        Elapsing1 += (sender, eventArgs) => func(sender, eventArgs);
        //    }
        //}



        public TimeSpan CurrentTime { get; private set; } = new TimeSpan();

        /// <summary>
        /// 每一週期的間隔時間 (msec)
        /// </summary>
        public TimeSpan Interval { get; }

        public Time() : this(new TimeSettings())
        {
        }

        public Time(TimeSettings timeSettings)
        {
            _timeSettings = timeSettings ?? throw new ArgumentNullException(nameof(timeSettings));

            if (_timeSettings.Interval < 0) throw new ArgumentOutOfRangeException(nameof(timeSettings.Interval));
            if (_timeSettings.StartDelay < 0) throw new ArgumentOutOfRangeException(nameof(timeSettings.StartDelay));
            if (_timeSettings.EventHandlersCount < 1) throw new ArgumentOutOfRangeException(nameof(timeSettings.EventHandlersCount));

            Interval = TimeSpan.FromMilliseconds(_timeSettings.Interval);

            EventHandlersCount = _timeSettings.EventHandlersCount;

            _countdownEvent = new CountdownEvent(EventHandlersCount);

            Initialize();

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
            var timeEventArgs = new TimeEventArgs()
            {
                CurrentTime = this.CurrentTime,
                NextTime = this.CurrentTime.Add(Interval),
            };

            _countdownEvent.Reset();

            _bufferBlock.Post(timeEventArgs);
            //_bufferBlock.Complete();

            //Task.WaitAll(_actionBlocks.Select(a => a.Completion).ToArray());

            _countdownEvent.Wait();

            this.CurrentTime = this.CurrentTime.Add(Interval);
        }

        private void Initialize()
        {
            _bufferBlock = new BufferBlock<TimeEventArgs>(new DataflowBlockOptions() { BoundedCapacity = 1 });
            _broadcaster = new BroadcastBlock<TimeEventArgs>(timeEventArgs => timeEventArgs);

            var dataflowLinkOptions = new DataflowLinkOptions() { PropagateCompletion = true };

            _bufferBlock.LinkTo(_broadcaster, dataflowLinkOptions);

            var actionExecutionDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1 };

            for (int i = 0; i < this.EventHandlersCount; i++)
            {
                var timeElapingEvent = new TimeElapsingEvent();
                TimeElapsingEventHandlers[i] = timeElapingEvent;

                var actionBlock = new ActionBlock<TimeEventArgs>((timeEventArgs) =>
                {
                    timeElapingEvent.Elapse(timeEventArgs);
                    //return Task.CompletedTask;
                    _countdownEvent.Signal();
                },
                actionExecutionDataflowBlockOptions);

                _broadcaster.LinkTo(
                    actionBlock,
                    dataflowLinkOptions);

                _actionBlocks.Add(actionBlock);
            }

            //_bufferBlock.Completion.ContinueWith(t => _broadcaster.Complete());

            //_broadcaster.Completion.ContinueWith(t =>
            //{
            //    //_actionBlocks.ForEach(x => x.Complete());
            //});
        }

        private void AutoElapse()
        {
            CheckSettings();

            _elaspeTask = Task.Run(() =>
            {
                do
                {
                    SpinWait.SpinUntil(() => _elapseSignal);

                    this.ElapseOnce();

                    Thread.Sleep(_timeSettings.Interval);
                } while (_timeSettings.SekaiSaigoNoHi > this.CurrentTime);
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

        public class TimeElapsingEvent : ITimeElapsingEvent
        {
            public event EventHandler<TimeEventArgs> Ready;
            public event EventHandler<TimeEventArgs> Elapsing;
            public event EventHandler<TimeEventArgs> Elapsed;

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

            public void Elapse(TimeEventArgs timeEventArgs)
            {
                OnReady(timeEventArgs);

                OnElapsing(timeEventArgs);

                OnElapsed(timeEventArgs);
            }
        }
    }
}
