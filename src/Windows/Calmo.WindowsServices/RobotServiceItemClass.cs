using System;
using System.Collections.Generic;
using Calmo.WindowsServices.Properties;

namespace Calmo.WindowsServices
{
    [Serializable]
    public class RobotServiceItemClass
    {
        public string Identificator { get; set; }

        private RobotServiceItemType _type = RobotServiceItemType.Synchronous;
        public RobotServiceItemType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public bool HasReturn { get; set; }

        private RobotServiceItemStatus _status = RobotServiceItemStatus.WaitingStart;
        public RobotServiceItemStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public Dictionary<string, object> Parameters { get; set; }
        public DateTime? ExecuteTime { get; set; }
        public TimeSpan? ExecuteInterval { get; set; }
        public DateTime? LastExecutionTime { get; set; }
        public DateTime CreateTime { get; set; }
        public string ErrorMessage { get; set; }

        private static int[] ShortTimeToPartsArray(string shortTime)
        {
            int[] returnArray;

            try
            {
                string[] parts = shortTime.Split(":".ToCharArray());

                if (parts.Length == 1)
                {
                    returnArray = new int[1];
                    returnArray[0] = Convert.ToInt32(parts[0]);
                }
                else if (parts.Length == 2)
                {
                    returnArray = new int[2];
                    returnArray[0] = Convert.ToInt32(parts[0]);
                    returnArray[1] = Convert.ToInt32(parts[1]);
                }
                else if (parts.Length == 3)
                {
                    returnArray = new int[3];
                    returnArray[0] = Convert.ToInt32(parts[0]);
                    returnArray[1] = Convert.ToInt32(parts[1]);
                    returnArray[2] = Convert.ToInt32(parts[2]);
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(Resources.InvalidTime, ex);
            }

            return returnArray;
        }

        public static DateTime GenerateExecuteTime(string shortTime)
        {
            int[] parts = ShortTimeToPartsArray(shortTime);

            int hour = parts[0];
            int minute = parts[1];
            int second = parts[2];

            return GenerateExecuteTime(hour, minute, second);
        }

        public static DateTime GenerateExecuteTime(int hour, int minute, int second)
        {
            return new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, hour, minute, second);
        }

        public static TimeSpan GenerateExecuteInterval(string shortTime)
        {
            int[] parts = ShortTimeToPartsArray(shortTime);

            int hours = parts[0];
            int minutes = parts[1];
            int seconds = parts[2];

            return GenerateExecuteInterval(hours, minutes, seconds);
        }

        public static TimeSpan GenerateExecuteInterval(int hours, int minutes, int seconds)
        {
            return new TimeSpan(hours, minutes, seconds);
        }
    }
}
