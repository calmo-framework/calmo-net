namespace ResourceIT.Forms.Controls.VideoPlayer
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    public sealed class StreamVideoSource : VideoSource
    {
        public static readonly BindableProperty StreamProperty = BindableProperty.Create("Stream",
            typeof(Func<CancellationToken, Task<Stream>>), typeof(StreamVideoSource));

        public override bool Equals(VideoSource other)
        {
            if (other is StreamVideoSource)
            {
                return ((StreamVideoSource) other).Stream.Equals(this.Stream);
            }
            return true;
        }

        [AsyncStateMachine(typeof(StreamAsyncStateMachine))]
        public Task<Stream> GetStreamAsync(CancellationToken userToken = new CancellationToken())
        {
            var meuOvo = new StreamAsyncStateMachine
            {
                _this = this,
                userToken = userToken,
                _builder = AsyncTaskMethodBuilder<Stream>.Create(),
                _state = -1
            };
            meuOvo._builder.Start(ref meuOvo);
            return meuOvo._builder.Task;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == StreamProperty.PropertyName)
                base.OnSourceChanged();

            if (propertyName != null)
                base.OnPropertyChanged(propertyName);
        }

        public string Format { get; set; }

        public Func<CancellationToken, Task<Stream>> Stream
        {
            get { return (Func<CancellationToken, Task<Stream>>) base.GetValue(StreamProperty); }
            set { base.SetValue(StreamProperty, value); }
        }

        private struct StreamAsyncStateMachine : IAsyncStateMachine
        {
            public int _state;
            public StreamVideoSource _this;
            public AsyncTaskMethodBuilder<Stream> _builder;
            private TaskAwaiter<Stream> _awaiter;
            private Stream _result;
            public CancellationToken userToken;

            public void MoveNext()
            {
                Stream stream;
                int num = this._state;
                try
                {
                    if (num != 0)
                    {
                        this._result = null;
                        if (this._this.Stream == null)
                        {
                            goto Label_00F4;
                        }
                        this._this.OnLoadingStarted();
                        this.userToken.Register(new Action(this._this.CancellationTokenSource.Cancel));
                    }
                    try
                    {
                        TaskAwaiter<Stream> awaiter;
                        if (num != 0)
                        {
                            awaiter = this._this.Stream(this._this.CancellationTokenSource.Token).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this._state = num = 0;
                                this._awaiter = awaiter;
                                this._builder.AwaitUnsafeOnCompleted(ref this._awaiter, ref this);
                                return;
                            }
                        }
                        else
                        {
                            awaiter = this._awaiter;
                            this._awaiter = new TaskAwaiter<Stream>();
                            this._state = num = -1;
                        }
                        Stream result = awaiter.GetResult();
                        awaiter = new TaskAwaiter<Stream>();
                        Stream stream2 = result;
                        this._result = stream2;
                        this._this.OnLoadingCompleted(false);
                    }
                    catch (OperationCanceledException)
                    {
                        this._this.OnLoadingCompleted(true);
                        throw;
                    }
                    Label_00F4:
                    stream = this._result;
                }
                catch (Exception exception)
                {
                    this._state = -2;
                    this._builder.SetException(exception);
                    return;
                }
                this._state = -2;
                this._builder.SetResult(stream);
            }
            
            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this._builder.SetStateMachine(stateMachine);
            }
        }

    }
}

