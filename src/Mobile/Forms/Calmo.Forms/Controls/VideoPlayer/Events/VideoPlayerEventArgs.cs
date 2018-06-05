namespace ResourceIT.Forms.Controls.VideoPlayer.Events
{
    using System;
    using System.Runtime.CompilerServices;

    public class VideoPlayerEventArgs : EventArgs
    {
        public VideoPlayerEventArgs()
        {
        }

        public VideoPlayerEventArgs(TimeSpan currentTime, TimeSpan duration, float rate)
        {
            this.CurrentTime = currentTime;
            this.Duration = duration;
            this.Rate = rate;
        }

        public TimeSpan CurrentTime { get; }

        public TimeSpan Duration { get; }

        public float Rate { get; }
    }
}

