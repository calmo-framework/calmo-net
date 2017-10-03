using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using ResourceIT.Forms.Controls.VideoPlayer;
using ResourceIT.Forms.Controls.VideoPlayer.Interfaces;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer.SourceHandlers
{
    public sealed class StreamVideoSourceHandler : IVideoSourceHandler
    {
        private string GetFileName(Stream stream, string format)
        {
            stream.Position = 0L;
            string str = Convert.ToBase64String(MD5.Create().ComputeHash(stream)).Replace("=", string.Empty).Replace(@"\", string.Empty).Replace("/", string.Empty);
            return $"{str}_temp.{format}";
        }

        [AsyncStateMachine(typeof(AsyncStateMachineLoadVideoAsync))]
        public Task<string> LoadVideoAsync(VideoSource source, CancellationToken cancellationToken = new CancellationToken())
        {
            var stateMachine = new AsyncStateMachineLoadVideoAsync
            {
                _this = this,
                source = source,
                cancellationToken = cancellationToken,
                _builder = AsyncTaskMethodBuilder<string>.Create(),
                _state = -1
            };
            stateMachine._builder.Start(ref stateMachine);
            return stateMachine._builder.Task;
        }

        [CompilerGenerated]
        private struct AsyncStateMachineLoadVideoAsync : IAsyncStateMachine
        {
            public int _state;
            public StreamVideoSourceHandler _this;
            public AsyncTaskMethodBuilder<string> _builder;
            private ConfiguredTaskAwaitable<Stream>.ConfiguredTaskAwaiter _configuredAwaiter;
            private TaskAwaiter _awaiter;
            private string _path;
            private Stream _stream;
            private StreamVideoSource _streamVideoSource;
            private FileStream _tempFile;
            public CancellationToken cancellationToken;
            public VideoSource source;

            public void MoveNext()
            {
                string str;
                int num = this._state;
                try
                {
                    ConfiguredTaskAwaitable<Stream>.ConfiguredTaskAwaiter awaiter;
                    switch (num)
                    {
                        case 0:
                            break;

                        case 1:
                            goto Label_00C3;

                        default:
                            this._path = null;
                            this._streamVideoSource = this.source as StreamVideoSource;
                            if (this._streamVideoSource?.Stream == null)
                            {
                                goto Label_01FB;
                            }
                            awaiter = this._streamVideoSource.GetStreamAsync(this.cancellationToken).ConfigureAwait(false).GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto Label_00AC;
                            }
                            this._state = num = 0;
                            this._configuredAwaiter = awaiter;
                            this._builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                            return;
                    }
                    awaiter = this._configuredAwaiter;
                    this._configuredAwaiter = new ConfiguredTaskAwaitable<Stream>.ConfiguredTaskAwaiter();
                    this._state = num = -1;
                Label_00AC:
                    Stream introduced9 = awaiter.GetResult();
                    awaiter = new ConfiguredTaskAwaitable<Stream>.ConfiguredTaskAwaiter();
                    Stream stream = introduced9;
                    this._stream = stream;
                Label_00C3:;
                    try
                    {
                        if (num != 1)
                        {
                            if (this._stream == null)
                            {
                                goto Label_01F4;
                            }
                            string fileName = this._this.GetFileName(this._stream, this._streamVideoSource.Format);
                            string str3 = Path.Combine(Path.GetTempPath(), "MediaCache");
                            this._path = Path.Combine(str3, fileName);
                            if (File.Exists(this._path))
                            {
                                goto Label_01F4;
                            }
                            if (!Directory.Exists(str3))
                            {
                                Directory.CreateDirectory(str3);
                            }
                            this._tempFile = File.Create(this._path);
                        }
                        try
                        {
                            TaskAwaiter awaiter2;
                            if (num != 1)
                            {
                                awaiter2 = this._stream.CopyToAsync(this._tempFile).GetAwaiter();
                                if (!awaiter2.IsCompleted)
                                {
                                    this._state = num = 1;
                                    this._awaiter = awaiter2;
                                    this._builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
                                    return;
                                }
                            }
                            else
                            {
                                awaiter2 = this._awaiter;
                                this._awaiter = new TaskAwaiter();
                                this._state = num = -1;
                            }
                            awaiter2.GetResult();
                            awaiter2 = new TaskAwaiter();
                        }
                        finally
                        {
                            if ((num < 0) && (this._tempFile != null))
                            {
                                this._tempFile.Dispose();
                            }
                        }
                        this._tempFile = null;
                    }
                    finally
                    {
                        if ((num < 0) && (this._stream != null))
                        {
                            this._stream.Dispose();
                        }
                    }
                Label_01F4:
                    this._stream = null;
                Label_01FB:
                    str = this._path;
                }
                catch (Exception exception)
                {
                    this._state = -2;
                    this._builder.SetException(exception);
                    return;
                }
                this._state = -2;
                this._builder.SetResult(str);
            }

            [DebuggerHidden]
            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this._builder.SetStateMachine(stateMachine);
            }
        }
    }
}

