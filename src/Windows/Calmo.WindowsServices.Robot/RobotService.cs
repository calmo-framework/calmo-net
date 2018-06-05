using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading;
using Calmo.Core;
using Calmo.WindowsServices.Robot.Configuration;

namespace Calmo.WindowsServices.Robot
{
    public partial class RobotService : ServiceBase
    {
        private RobotConfiguration _configuration;
        public static Dictionary<String, DateTime> Executing;

        public RobotService()
        {
            Executing = new Dictionary<String, DateTime>();
#if !DEBUG
            InitializeComponent();
#endif
            _configuration = CustomConfiguration.Settings.Robot();
            this.ServiceName = _configuration.ServiceName;
#if DEBUG
            tmrRobot = new System.Timers.Timer();
            this.tmrRobot_Elapsed(null, null);
#endif
        }

        protected override void OnStart(string[] args)
        {
            LogStep("Robot Service started.");
            tmrRobot.Enabled = true;
            tmrRobot.Interval = 1000;
            tmrRobot.Start();
        }

        protected override void OnStop()
        {
            LogStep("Robot Service stopped.");
            tmrRobot.Enabled = false;
            tmrRobot.Stop();
        }

        protected override void OnPause()
        {
            LogStep("Robot Service paused.");
            base.OnPause();
            tmrRobot.Enabled = false;
            tmrRobot.Stop();
        }

        protected override void OnContinue()
        {
            LogStep("Robot Service restarted.");
            base.OnContinue();
            tmrRobot.Enabled = true;
            tmrRobot.Start();
        }

        private void tmrRobot_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                LogStep("Robot Service elapsed method triggered.");

                var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                _configuration = CustomConfiguration.Settings.Robot(LogStep);

                tmrRobot.Interval = _configuration.ExecutionMilliseconds;

                LogStep("Robot Service configuration updated.");

                foreach (var def in _configuration.ServiceClasses)
                {
                        var serviceClassManager = new RobotServiceClassManager(def, appPath);

                        serviceClassManager.Error += serviceClassManager_Error;
                        serviceClassManager.Step += serviceClassManager_Step;

                        var thread = new Thread(serviceClassManager.Execute);

                        thread.Start();
                    }
                }
            catch (Exception exp)
            {
                LogException(exp);
            }
        }

        private void serviceClassManager_Step(string stepMessage)
        {
            LogStep(stepMessage);
        }

        private void serviceClassManager_Error(Exception exception)
        {
            LogException(exception);
        }

        private void LogException(Exception exception)
        {
            try
            {
                var message = this.GetErrorMessage(exception);

                if (_configuration.LogTextFile)
                    this.WriteTextFile(message);

                EventLog.WriteEntry(message, EventLogEntryType.Error);
            }
            catch
            {
                // ignored
            }
        }

        private void LogStep(string stepMessage)
        {
            if (!_configuration.LogSteps) return;

            try
            {
                var message = this.GetLogMessage(stepMessage);

                if (_configuration.LogTextFile)
                    this.WriteTextFile(message);

                EventLog.WriteEntry(stepMessage, EventLogEntryType.Information);
            }
            catch
            {
                // ignored
            }
        }

        private string GetLogMessage(string message)
        {
            if (Thread.CurrentThread.CurrentCulture.Name.ToLower().Contains("pt"))
                return $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - {message}";

            return $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} - {message}";
        }

        private string GetErrorMessage(Exception exception)
        {
            var error = new StringBuilder();

            while (exception != null)
            {
                error.AppendLine(String.Concat(exception.Message, Environment.NewLine, exception.StackTrace));

                exception = exception.InnerException;
            }

            return this.GetLogMessage(error.ToString());
        }

        private void WriteTextFile(string text)
        {
            if (!Directory.Exists(_configuration.TextFilePath))
                Directory.CreateDirectory(_configuration.TextFilePath);

            var filePath = Path.Combine(_configuration.TextFilePath,
                $"RobotServiceLog_{DateTime.Today.ToString("yyyyMMdd")}.txt");

            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(text);
            }
        }
    }
}