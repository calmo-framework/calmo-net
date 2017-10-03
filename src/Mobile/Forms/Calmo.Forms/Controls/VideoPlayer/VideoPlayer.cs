using ResourceIT.Forms.Controls.VideoPlayer.Events;
using ResourceIT.Forms.Controls.VideoPlayer.Interfaces;

namespace ResourceIT.Forms.Controls.VideoPlayer
{
    using ResourceIT.Forms.Controls.VideoPlayer.Constants;
    using ResourceIT.Forms.Controls.VideoPlayer.Diagnostics;
    using Controls.VideoPlayer.Events;
    using Controls.VideoPlayer.ExtensionMethods;
    using Controls.VideoPlayer.Interfaces;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class VideoPlayer : View
    {
        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create("AutoPlay", typeof(bool), typeof(VideoPlayer), false);
        public static readonly BindableProperty CurrentTimeProperty = CurrentTimePropertyKey.BindableProperty;
        internal static readonly BindablePropertyKey CurrentTimePropertyKey = BindableProperty.CreateReadOnly("CurrentTime", typeof(TimeSpan), typeof(VideoPlayer), TimeSpan.Zero);
        public static readonly BindableProperty DisplayControlsProperty = BindableProperty.Create("DisplayControls", typeof(bool), typeof(VideoPlayer), true);
        public static readonly BindableProperty FillModeProperty = BindableProperty.Create("FillMode", typeof(FillMode), typeof(VideoPlayer), FillMode.ResizeAspect);
        public static readonly BindableProperty IsLoadingProperty = IsLoadingPropertyKey.BindableProperty;
        public static readonly BindablePropertyKey IsLoadingPropertyKey = BindableProperty.CreateReadOnly("IsLoading", typeof(bool), typeof(VideoPlayer), false);
        internal IVideoPlayerRenderer NativeRenderer;
        public static readonly BindableProperty PauseCommandProperty = BindableProperty.Create("PauseCommand", typeof(ICommand), typeof(VideoPlayer));
        public static readonly BindableProperty PlayCommandProperty = BindableProperty.Create("PlayCommand", typeof(ICommand), typeof(VideoPlayer));
        public static readonly BindableProperty RepeatProperty = BindableProperty.Create("Repeat", typeof(bool), typeof(VideoPlayer), false);
        public static readonly BindableProperty SeekCommandProperty = BindableProperty.Create("SeekCommand", typeof(ICommand), typeof(VideoPlayer));
        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(VideoSource), typeof(VideoPlayer));
        public static readonly BindableProperty StateProperty = StatePropertyKey.BindableProperty;
        internal static readonly BindablePropertyKey StatePropertyKey = BindableProperty.CreateReadOnly("State", typeof(PlayerState), typeof(VideoPlayer), PlayerState.Idle);
        public static readonly BindableProperty TimeElapsedIntervalProperty = BindableProperty.Create("TimeElapsedInterval", typeof(int), typeof(VideoPlayer), 0);
        public static readonly BindableProperty VolumeProperty = BindableProperty.Create("Volume", typeof(int), typeof(VideoPlayer), -2147483648);

        [field: CompilerGenerated]
        public event EventHandler<VideoPlayerEventArgs> Completed;

        [field: CompilerGenerated]
        public event EventHandler<VideoPlayerErrorEventArgs> Failed;

        [field: CompilerGenerated]
        public event EventHandler<VideoPlayerEventArgs> Paused;

        [field: CompilerGenerated]
        public event EventHandler<VideoPlayerStateChangedEventArgs> PlayerStateChanged;

        [field: CompilerGenerated]
        public event EventHandler<VideoPlayerEventArgs> Playing;

        [field: CompilerGenerated]
        public event EventHandler<VideoPlayerEventArgs> TimeElapsed;

        public VideoPlayer()
        {
            LayoutOptions options;
            object[] objArray1 = { base.Id };
            Log.Info($"Creating video player instance [{(object[]) objArray1}]");
            base.HeightRequest = 300.0;
            base.VerticalOptions = options = LayoutOptions.FillAndExpand;
            base.HorizontalOptions = options;

            this.PlayCommand = new Command(() => {
                if (this.NativeRenderer != null)
                {
                    this.NativeRenderer.Play();
                }
                else
                {
                    IVideoPlayerRenderer nativeRenderer = this.NativeRenderer;
                }
            }, () => (this.NativeRenderer != null) && this.NativeRenderer.CanPlay());
            this.PauseCommand = new Command(() => {
                if (this.NativeRenderer != null)
                {
                    this.NativeRenderer.Pause();
                }
                else
                {
                    IVideoPlayerRenderer nativeRenderer = this.NativeRenderer;
                }
            }, () => (this.NativeRenderer != null) && this.NativeRenderer.CanPause());
            this.SeekCommand = new Command<string>(delegate (string time) {
                if (this.NativeRenderer != null)
                {
                    this.NativeRenderer.Seek(int.Parse(time));
                }
                else
                {
                    IVideoPlayerRenderer nativeRenderer = this.NativeRenderer;
                }
            }, delegate (string time) {
                return (this.NativeRenderer != null) && this.NativeRenderer.CanSeek(int.Parse(time));
            });
        }

        public VideoPlayer(VideoSource source, bool autoPlay = false)
        {
            this.AutoPlay = autoPlay;
            this.Source = source;
        }

        public VideoPlayer(string source, bool autoPlay = false)
        {
            this.AutoPlay = autoPlay;
            this.Source = source;
        }

        internal void OnCompleted(VideoPlayerEventArgs e)
        {
            this.Completed.RaiseEvent<VideoPlayerEventArgs>(this, e);
            this.OnPlayerStateChanged(new VideoPlayerStateChangedEventArgs(e, PlayerState.Completed));
        }

        public void OnFailed(VideoPlayerErrorEventArgs e)
        {
            Exception errorObject = e.ErrorObject as Exception;
            if (errorObject != null)
            {
                Log.Error(errorObject, e.Message);
            }
            else
            {
                Log.Error(e.Message);
            }
            this.Failed.RaiseEvent<VideoPlayerErrorEventArgs>(this, e);
            this.OnPlayerStateChanged(new VideoPlayerStateChangedEventArgs(e, PlayerState.Error));
        }

        public void OnPaused(VideoPlayerEventArgs e)
        {
            this.Paused.RaiseEvent<VideoPlayerEventArgs>(this, e);
            this.OnPlayerStateChanged(new VideoPlayerStateChangedEventArgs(e, PlayerState.Paused));
        }

        public void OnPlayerStateChanged(VideoPlayerStateChangedEventArgs e)
        {
            if (e != null)
            {
                PlayerState currentState = e.CurrentState;
                base.SetValue(StatePropertyKey, e.CurrentState);
                this.PlayerStateChanged.RaiseEvent<VideoPlayerStateChangedEventArgs>(this, e);
            }
        }

        public void OnPlaying(VideoPlayerEventArgs e)
        {
            this.Playing.RaiseEvent<VideoPlayerEventArgs>(this, e);
            this.OnPlayerStateChanged(new VideoPlayerStateChangedEventArgs(e, PlayerState.Playing));
        }

        internal void OnTimeElapsed(VideoPlayerEventArgs e)
        {
            this.TimeElapsed.RaiseEvent<VideoPlayerEventArgs>(this, e);
        }

        public void Pause()
        {
            this.PauseCommand.Execute(null);
        }

        public void Play()
        {
            this.PlayCommand.Execute(null);
        }

        public void Seek(int time)
        {
            this.SeekCommand.Execute(((int) time).ToString());
        }

        public bool AutoPlay
        {
            get { return (bool) base.GetValue(AutoPlayProperty); }
            set { base.SetValue(AutoPlayProperty, value); }
        }

        public TimeSpan CurrentTime => (TimeSpan) base.GetValue(CurrentTimeProperty);

        public bool DisplayControls
        {
            get { return (bool) base.GetValue(DisplayControlsProperty); }
            set { base.SetValue(DisplayControlsProperty, (bool) value); }
        }

        public FillMode FillMode
        {
            get { return (FillMode) base.GetValue(FillModeProperty); }
            set { base.SetValue(FillModeProperty, value); }
        }

        public bool IsLoading => (bool) base.GetValue(IsLoadingProperty);

        public ICommand PauseCommand
        {
            get { return (ICommand) base.GetValue(PauseCommandProperty); }
            private set { base.SetValue(PauseCommandProperty, value); }
        }

        public ICommand PlayCommand
        {
            get { return (ICommand) base.GetValue(PlayCommandProperty); }
            private set { base.SetValue(PlayCommandProperty, value); }
        }

        public bool Repeat
        {
            get { return (bool) base.GetValue(RepeatProperty); }
            set { base.SetValue(RepeatProperty, value); }
        }

        public ICommand SeekCommand
        {
            get { return (ICommand) base.GetValue(SeekCommandProperty); }
            private set { base.SetValue(SeekCommandProperty, value); }
        }

        [TypeConverter(typeof(VideoSourceConverter))]
        public VideoSource Source
        {
            get { return (VideoSource) base.GetValue(SourceProperty); }
            set { base.SetValue(SourceProperty, value); }
        }

        public PlayerState State => (PlayerState) base.GetValue(StateProperty);

        public int TimeElapsedInterval
        {
            get { return (int) base.GetValue(TimeElapsedIntervalProperty); }
            set
            {
                if (value >= 0)
                    base.SetValue(TimeElapsedIntervalProperty, (int) value);
                else
                {
                    var objArray1 = new object[] {(int) value};
                    var message =  $"TimeElapsedInterval of '{objArray1}' must be greater than or equal to 0 seconds.";
                    this.Failed.RaiseEvent(new VideoPlayerErrorEventArgs(message));
                }
            }
        }

        public int Volume
        {
            get { return (int) base.GetValue(VolumeProperty); }
            set { base.SetValue(VolumeProperty, (int) value); }
        }

        
        private sealed class MEUOVO
        {
            public static readonly VideoPlayer.MEUOVO LALALA = new VideoPlayer.MEUOVO();
            public static EventHandler<VideoPlayerEventArgs> eventHandler;

            internal void MEUOVOEvent(object sender, VideoPlayerEventArgs args)
            {
                if (args.CurrentTime.TotalSeconds > 20.0)
                {
                    DependencyService.Get<IPlatformFeatures>(DependencyFetchTarget.GlobalInstance).Exit();
                }
            }
        }
    }
}

