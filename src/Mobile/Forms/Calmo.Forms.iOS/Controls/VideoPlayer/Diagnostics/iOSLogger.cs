using System;
using ResourceIT.Forms.Controls.VideoPlayer.Diagnostics;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer.Diagnostics
{
    internal class iOSLogger : ILogger
    {
        private string _logEntryPrefix = "VideoPlayer";

        public void Error(Exception exception)
        {
            Console.WriteLine($"{this._logEntryPrefix} [ERROR]: {exception.Message}{Environment.NewLine}{exception.StackTrace}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"{this._logEntryPrefix} [ERROR]: {message}");
        }

        public void Error(Exception exception, string message)
        {
            Console.WriteLine($"{this._logEntryPrefix} [ERROR]: {message}{Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}");
        }

        public void Info(string message)
        {
            Console.WriteLine($"{this._logEntryPrefix} [INFO]: {message}");
        }

        public void Warn(string message)
        {
            Console.WriteLine($"{this._logEntryPrefix} [WARN]: {message}");
        }
    }
}

