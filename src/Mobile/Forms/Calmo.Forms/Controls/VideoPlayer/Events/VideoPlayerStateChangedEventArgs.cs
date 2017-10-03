namespace ResourceIT.Forms.Controls.VideoPlayer.Events
{
    using ResourceIT.Forms.Controls.VideoPlayer.Constants;
    using System;
    using System.Runtime.CompilerServices;

    public class VideoPlayerStateChangedEventArgs : VideoPlayerEventArgs
    {
        public VideoPlayerStateChangedEventArgs(PlayerState currentState)
        {
            this.CurrentState = currentState;
        }

        public VideoPlayerStateChangedEventArgs(VideoPlayerEventArgs videoPlayerEventArgs, PlayerState currentState) : this(videoPlayerEventArgs.CurrentTime, videoPlayerEventArgs.Duration, videoPlayerEventArgs.Rate, currentState)
        {
        }

        public VideoPlayerStateChangedEventArgs(TimeSpan currentTime, TimeSpan duration, float rate, PlayerState currentState) : base(currentTime, duration, rate)
        {
            this.CurrentState = currentState;
        }

        public PlayerState CurrentState { get; private set; }
    }
}

