namespace ResourceIT.Forms.Controls.VideoPlayer.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class VideoPlayerErrorEventArgs : VideoPlayerEventArgs
    {
        public VideoPlayerErrorEventArgs(string message, object errorObject = null)
        {
            this.Message = message;
            this.ErrorObject = errorObject;
        }

        public VideoPlayerErrorEventArgs(VideoPlayerEventArgs videoPlayerEventArgs, string message, object errorObject) : this(videoPlayerEventArgs.CurrentTime, videoPlayerEventArgs.Duration, videoPlayerEventArgs.Rate, message, errorObject)
        {
        }

        public VideoPlayerErrorEventArgs(TimeSpan currentTime, TimeSpan duration, float rate, string message, object errorObject) : base(currentTime, duration, rate)
        {
            this.Message = message;
            this.ErrorObject = errorObject;
        }

        public object ErrorObject { get; private set; }

        public string Message { get; private set; }
    }
}

