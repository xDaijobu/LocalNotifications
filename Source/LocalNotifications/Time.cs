using System;
using Newtonsoft.Json;

namespace LocalNotifications
{
    public class Time
    {
        public int Hour { get; }
        public int Minute { get; }
        public int Second { get; }

        /// <summary>
        /// Hour &lt; 23 , Minute &lt; 59 , Second &lt; 59 | 
        /// </summary>
        [JsonConstructor]
        public Time(int hour = 0, int minute = 0, int second = 0)
        {
            Validation(hour, minute, second);

            Hour = hour;
            Minute = minute;
            Second = second;
        }

        /// <summary>
        /// </summary>
        /// <param name="time">HH24:MM:ss</param>
        public Time(string time)
        {
            if (string.IsNullOrEmpty(time))
                throw new NullReferenceException("time");

            TimeSpan span = TimeSpan.Parse(time);

            Validation(span.Hours, span.Minutes, span.Seconds);

            Hour = span.Hours;
            Minute = span.Minutes;
            Second = span.Seconds;
        }

        private void Validation(int hour, int minute, int second)
        {
            if (hour >= 24)
                throw new ArgumentOutOfRangeException("Accepted range is 0 to 23 inclusive.");
            if (minute >= 60)
                throw new ArgumentOutOfRangeException("Accepted range is 0 to 59 inclusive.");
            if (second >= 60)
                throw new ArgumentOutOfRangeException("Accepted range is 0 to 59 inclusive.");
        }
    }
}
