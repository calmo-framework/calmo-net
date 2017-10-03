namespace ResourceIT.Forms.Controls.VideoPlayer.Diagnostics
{
    using System;

    public interface ILogger
    {
        void Error(Exception exception);
        void Error(string message);
        void Error(Exception exception, string message);
        void Info(string message);
        void Warn(string message);
    }
}

