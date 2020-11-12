using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GameHost1.Universes.Evance.Milestone3
{
    public class Time : IDisposable
    {
        private TimeSettings _timeSettings;
        private Task _elaspeTask;
        private volatile bool _elapseSignal = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool isDisposed;

        private BufferBlock<TimeEventArgs> _bufferBlock;
        private TransformBlock<TimeEventArgs, TimeEventArgs> _timeReadyTransformBlock;
        private BroadcastBlock<TimeEventArgs> _broadcaster;
        private List<TransformBlock<TimeEventArgs, TimeEventArgs>> _timeElapingTransformBlocks = new List<TransformBlock<TimeEventArgs, TimeEventArgs>>();
        private BatchBlock<TimeEventArgs> _timeElapingFinishedBatchBlock;
        private ActionBlock<TimeEventArgs[]> _timeElapsedActionBlock;

        private AutoResetEvent _completeAllTimeEventsSignal = new AutoResetEvent(false);

        public int EventHandlersCount { get; private set; }

        public ConcurrentDictionary<int, TimeElapsingEvent> TimeElapsingEventHandlers { get; private set; } = new ConcurrentDictionary<int, TimeElapsingEvent>();

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

            _bufferBlock.Post(timeEventArgs);

            _completeAllTimeEventsSignal.WaitOne();

            this.CurrentTime = this.CurrentTime.Add(Interval);
        }

        private void Initialize()
        {
            #region create blocks

            _bufferBlock = new BufferBlock<TimeEventArgs>(new DataflowBlockOptions() { BoundedCapacity = 1 });

            _timeReadyTransformBlock = new TransformBlock<TimeEventArgs, TimeEventArgs>(timeEventArgs =>
            {
                TimeElapsingEventHandlers[0].OnReady(timeEventArgs);

                return timeEventArgs;
            });

            _broadcaster = new BroadcastBlock<TimeEventArgs>(timeEventArgs => timeEventArgs);

            var actionExecutionDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1 };

            for (int i = 0; i < this.EventHandlersCount; i++)
            {
                var timeElapingEvent = new TimeElapsingEvent();
                TimeElapsingEventHandlers[i] = timeElapingEvent;

                var timeElapsingTransformBlocks = new TransformBlock<TimeEventArgs, TimeEventArgs>((timeEventArgs) =>
                {
                    timeElapingEvent.OnElapsing(timeEventArgs);

                    return timeEventArgs;
                },
                actionExecutionDataflowBlockOptions);

                _timeElapingTransformBlocks.Add(timeElapsingTransformBlocks);
            }

            _timeElapingFinishedBatchBlock = new BatchBlock<TimeEventArgs>(this.EventHandlersCount, new GroupingDataflowBlockOptions()
            {
                //// 在您必須以不可部分完成的方式協調來自多個來源之消耗時，可以使用非窮盡模式。 在 BatchBlock<T> 建構函式的 dataflowBlockOptions 參數中，設定 Greedy 為 False，來指定非窮盡模式。
                //Greedy = false,
                // 非貪婪模式似乎有 bug (deadlock) 的情況，這邊的情境用貪婪模式就可以了，可參考 https://stackoverflow.com/questions/29861230/tpl-dataflow-broadcastblock-to-batchblocks
                Greedy = true,
            });

            _timeElapsedActionBlock = new ActionBlock<TimeEventArgs[]>(timeEventArgsCollection =>
            {
                TimeElapsingEventHandlers[0].OnElapsed(timeEventArgsCollection[0]);

                _completeAllTimeEventsSignal.Set();
            });

            #endregion

            #region link blocks

            var dataflowLinkOptions = new DataflowLinkOptions() { PropagateCompletion = true };

            _bufferBlock.LinkTo(_timeReadyTransformBlock, dataflowLinkOptions);
            _timeReadyTransformBlock.LinkTo(_broadcaster, dataflowLinkOptions);
            _timeElapingTransformBlocks.ForEach(b =>
            {
                _broadcaster.LinkTo(b, dataflowLinkOptions);
                b.LinkTo(_timeElapingFinishedBatchBlock, dataflowLinkOptions);
            });
            _timeElapingFinishedBatchBlock.LinkTo(_timeElapsedActionBlock, dataflowLinkOptions);

            #endregion
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

            internal protected virtual void OnReady(TimeEventArgs e)
            {
                Ready?.Invoke(this, e);
            }

            internal protected virtual void OnElapsing(TimeEventArgs e)
            {
                Elapsing?.Invoke(this, e);
            }

            internal protected virtual void OnElapsed(TimeEventArgs e)
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
