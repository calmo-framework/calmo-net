namespace ResourceIT.Forms.Controls.VideoPlayer.Interfaces
{
    using System;

    public interface IVideoPlayerRenderer
    {
        bool CanPause();
        bool CanPlay();
        bool CanSeek(int time);
        bool CanStop();
        void Pause();
        void Play();
        void Seek(int seekTime);
        void Stop();
    }
}

