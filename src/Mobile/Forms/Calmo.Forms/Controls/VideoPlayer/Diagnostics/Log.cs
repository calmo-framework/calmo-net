namespace ResourceIT.Forms.Controls.VideoPlayer.Diagnostics
{
    using System;
    using Xamarin.Forms;

    internal static class Log
    {
        public static void Error(Exception exception)
        {
            ILogger local1 = DependencyService.Get<ILogger>(0);
            if (local1 != null)
            {
                local1.Error(exception);
            }
        }

        public static void Error(string message)
        {
            ILogger local1 = DependencyService.Get<ILogger>(0);
            if (local1 != null)
            {
                local1.Error(message);
            }
        }

        public static void Error(Exception exception, string message)
        {
            ILogger local1 = DependencyService.Get<ILogger>(0);
            if (local1 != null)
            {
                local1.Error(exception, message);
            }
        }

        public static void Info(string message)
        {
            ILogger local1 = DependencyService.Get<ILogger>(0);
            if (local1 != null)
            {
                local1.Info(message);
            }
        }

        public static void Warn(string message)
        {
            ILogger local1 = DependencyService.Get<ILogger>(0);
            if (local1 != null)
            {
                local1.Warn(message);
            }
        }
    }
}

