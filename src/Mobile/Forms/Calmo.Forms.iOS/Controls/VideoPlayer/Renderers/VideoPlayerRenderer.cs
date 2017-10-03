using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AVFoundation;
using ResourceIT.Forms.Controls.VideoPlayer.Interfaces;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using AVKit;
using CoreMedia;
using ResourceIT.Forms.Controls.VideoPlayer;
using ResourceIT.Forms.Controls.VideoPlayer.Constants;
using ResourceIT.Forms.Controls.VideoPlayer.Events;
using Xamarin.Forms;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer, UIView>,
        IVideoPlayerRenderer
    {
        // Fields
        private NSObject _currentTimeObserver;
        private NSObject _didPlayToEndTimeNotificationObserver;
        private bool _isDisposed;
        private NSObject _periodicTimeOberserver;
        private AVPlayerViewController _playerControl;

        // Methods
        public VideoPlayerRenderer()
        {
        }

        public virtual bool CanPause()
        {
            return (this._playerControl.Player.Status == AVPlayerStatus.ReadyToPlay);
        }

        public virtual bool CanPlay()
        {
            return (this._playerControl.Player.Status == AVPlayerStatus.ReadyToPlay);
        }

        public virtual bool CanSeek(int time)
        {
            if (!this._playerControl.Player.CurrentItem.CanStepBackward)
            {
                return this._playerControl.Player.CurrentItem.CanStepForward;
            }
            return true;
        }

        public virtual bool CanStop()
        {
            return (this._playerControl.Player.Status == AVPlayerStatus.ReadyToPlay);
        }

        private VideoPlayerErrorEventArgs CreateVideoPlayerErrorEventArgs()
        {
            NSError errorObject = this._playerControl.Player.Error;
            if (errorObject != null)
            {
                return new VideoPlayerErrorEventArgs(this.CreateVideoPlayerEventArgs(), Enum.GetName(typeof(AVError), errorObject.Code), errorObject);
            }
            return null;
        }

        private VideoPlayerEventArgs CreateVideoPlayerEventArgs()
        {
            AVPlayerItem currentItem = this._playerControl.Player.CurrentItem;
            if (currentItem != null)
            {
                double num = !double.IsNaN(currentItem.Duration.Seconds) ? currentItem.Duration.Seconds : 0.0;
                return new VideoPlayerEventArgs(TimeSpan.FromSeconds(!double.IsNaN(currentItem.CurrentTime.Seconds) ? currentItem.CurrentTime.Seconds : 0.0), TimeSpan.FromSeconds(num), this._playerControl.Player.Rate);
            }
            return null;
        }

        private VideoPlayerStateChangedEventArgs CreateVideoPlayerStateChangedEventArgs(PlayerState state)
        {
            return new VideoPlayerStateChangedEventArgs(this.CreateVideoPlayerEventArgs(), state);
        }

        private void DidPlayToEndTimeNotification(NSNotification obj);
        protected override void Dispose(bool disposing);
        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            AVPlayer player = (this._playerControl == null) ? null : this._playerControl.Player;
            if (string.Equals((string)keyPath, "status"))
            {
                if ((player.Status == AVPlayerStatus.ReadyToPlay) && (player.CurrentItem.Status == AVPlayerItemStatus.ReadyToPlay))
                {
                    base.Element.OnPlayerStateChanged(this.CreateVideoPlayerStateChangedEventArgs(PlayerState.Prepared));
                    if (base.Element.AutoPlay && (player.CurrentItem.CurrentTime == CMTime.Zero))
                    {
                        this.Play();
                    }
                }
                else if ((player != null) && (player.Status == AVPlayerStatus.Failed))
                {
                    base.Element.OnFailed(this.CreateVideoPlayerErrorEventArgs());
                }
                base.Element.SetValue(ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer.IsLoadingPropertyKey, false);
            }
            else if (string.Equals((string)keyPath, "rate"))
            {
                if (player.Rate >= 1E-06)
                {
                    base.Element.OnPlaying(this.CreateVideoPlayerEventArgs());
                }
                else
                {
                    CMTime currentTime = player.CurrentItem.CurrentTime;
                    CMTime duration = player.CurrentItem.Duration;
                    if ((currentTime != CMTime.Zero) && (currentTime != duration))
                    {
                        base.Element.OnPaused(this.CreateVideoPlayerEventArgs());
                    }
                }
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }




        [AsyncStateMachine(typeof(<
        
        OnElementChanged

    >
        d__15
    ))]

        protected override void OnElementChanged(
            ElementChangedEventArgs<ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer> e);

        [AsyncStateMachine(typeof(OnElementPropertyChangedAsyncStateMachine))]
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var stateMachine = new OnElementPropertyChangedAsyncStateMachine();
            stateMachine._this = this;
            stateMachine.sender = sender;
            stateMachine.e = e;
            stateMachine._builder = AsyncVoidMethodBuilder.Create();
            stateMachine._state = -1;
            stateMachine._builder.Start(ref stateMachine);
        }

        public virtual void Pause();
        public virtual void Play();
        private void RegisterEvents();
        public virtual void Seek(int seekTime);
        public virtual void Stop();
        private void UnRegisterEvents();
        private void UpdateDisplayControls();
        private void UpdateFillMode();

        [AsyncStateMachine(typeof(<
        
        UpdateSource

    >
        d__23
    ))]
        private Task UpdateSource(ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer oldElement = null);
        private void UpdateTimeElapsedInterval();
        private void UpdateVisibility();
        private void UpdateVolume();

        // Properties
        protected AVPlayerViewController PlayerControl { get; }


        private struct OnElementPropertyChangedAsyncStateMachine : IAsyncStateMachine
        {
            // Fields
            public int _state;
            public VideoPlayerRenderer _this;
            public AsyncVoidMethodBuilder _builder;
            private TaskAwaiter _awaiter;
            public PropertyChangedEventArgs e;
            public object sender;

            // Methods
            void IAsyncStateMachine.MoveNext()
            {
                int num = this._state;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        this._this.OnElementPropertyChanged(this.sender, this.e);
                        if (((this._this.Element != null) && (this._this.Control != null)) &&
                            (((this._this._playerControl == null) ? null : this._this._playerControl.Player) != null))
                        {
                            if (this.e.PropertyName !=
                                ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer.DisplayControlsProperty.PropertyName)
                            {
                                if (this.e.PropertyName !=
                                    ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer.FillModeProperty.PropertyName)
                                {
                                    if (this.e.PropertyName !=
                                        ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer.TimeElapsedIntervalProperty
                                            .PropertyName)
                                    {
                                        if (this.e.PropertyName !=
                                            ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer.VolumeProperty
                                                .PropertyName)
                                        {
                                            if (this.e.PropertyName ==
                                                ResourceIT.Forms.Controls.VideoPlayer.VideoPlayer.SourceProperty
                                                    .PropertyName)
                                            {
                                                awaiter = this._this.UpdateSource(null).GetAwaiter();
                                                if (!awaiter.IsCompleted)
                                                {
                                                    this._state = num = 0;
                                                    this._awaiter = awaiter;
                                                    this._builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                                                    return;
                                                }
                                                goto Label_0182;
                                            }
                                            goto Label_0193;
                                        }
                                        this._this.UpdateVolume();
                                    }
                                    else
                                    {
                                        this._this.UpdateTimeElapsedInterval();
                                    }
                                }
                                else
                                {
                                    this._this.UpdateFillMode();
                                }
                            }
                            else
                            {
                                this._this.UpdateDisplayControls();
                            }
                        }
                        goto Label_01D3;
                    }
                    awaiter = this._awaiter;
                    this._awaiter = new TaskAwaiter();
                    this._state = num = -1;
                    Label_0182:
                    awaiter.GetResult();
                    awaiter = new TaskAwaiter();
                    goto Label_01D3;
                    Label_0193:
                    if (this.e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
                    {
                        this._this.UpdateVisibility();
                    }
                }
                catch (Exception exception)
                {
                    this._state = -2;
                    this._builder.SetException(exception);
                    return;
                }
                Label_01D3:
                this._state = -2;
                this._builder.SetResult();
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this._builder.SetStateMachine(stateMachine);
            }
        }
    }
}

