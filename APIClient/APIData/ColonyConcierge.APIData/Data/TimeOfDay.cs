using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Home-grown class for representing a time-of-day. Made to be JSON/REST-friendly
    /// Note that this class is typically "time zone agnostic" in that it is for logic that 
    /// does not depend a specific moment in time, but is for general times.
    /// For a specific moment in time, use the <see cref="TimeStamp"/> class.
    /// </summary>
    public struct TimeOfDay
    {
        /// <summary>
        /// The hour for this time, in 24-hour (0-23) format.
        /// </summary>
        public int Hour
        {
            get;
            /*private*/ set;
        }

        /// <summary>
        /// The Minute for this time, 0-59
        /// </summary>
        public int Minute
        {
            get;
            /*private*/ set;
        }

        /// <summary>
        /// he second for this time, 0-59
        /// </summary>
        public int Second
        {
            get;
            /*private*/ set;
        }

        //public TimeOfDay()
        //{

        //}

        public TimeOfDay(int hour, int minute, int second)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public static TimeOfDay From12Hour(bool pm, int hour, int minute, int second)
        {
            TimeOfDay result = new TimeOfDay()
            {
                Minute = minute,
                Second = second
            };

            if (hour < 12)
            {
                if (pm)
                {
                    result.Hour = hour + 12;
                }
                else
                {
                    result.Hour = hour - 1;
                }
            }
            else if (hour == 12)
            {
                if (pm)
                {
                    result.Hour = 12;
                }
                else
                {
                    result.Hour = 0;
                }
            }
            else
            {
                throw new InvalidOperationException($"Hour '{hour}' outside of valid range for 12 hour time.");
            }

            return result;
        }

        #region Conversion Operators
        

        public static implicit operator TimeOfDay(TimeSpan source)
        {
            TimeOfDay newValue = new TimeOfDay(source.Hours, source.Minutes, source.Seconds);
            
            return newValue;
        }

        public static implicit operator TimeSpan(TimeOfDay source)
        {
            return  new TimeSpan(source.Hour, source.Minute, source.Second);
        }

        public static implicit operator TimeSpan? (TimeOfDay? source)
        {
            return source.HasValue ? new TimeSpan(source.Value.Hour, source.Value.Minute, source.Value.Second) : (TimeSpan?)null;
        }

        public static implicit operator TimeOfDay(DateTime source)
        {
            TimeOfDay newValue = new TimeOfDay();
            if (source.Kind == DateTimeKind.Unspecified)
            {
                var tempValue = new DateTime(source.Ticks, DateTimeKind.Utc);
                newValue = new TimeOfDay(tempValue.Hour, tempValue.Minute, tempValue.Second);
            }
            else
            {
                var tempValue = source.ToUniversalTime();

                newValue = new TimeOfDay(tempValue.Hour, tempValue.Minute, tempValue.Second);
            }
            return newValue;
        }
        //TODO make this an explicit operator, or get rid of it?
        public static implicit operator DateTime(TimeOfDay source)
        {

            DateTime now = DateTime.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day, source.Hour, source.Minute, source.Second, DateTimeKind.Utc);
        }

        public static implicit operator DateTime? (TimeOfDay? source)
        {
            DateTime now = DateTime.UtcNow;
            return source.HasValue ? new DateTime(now.Year, now.Month, now.Day, source.Value.Hour, source.Value.Minute, source.Value.Second, DateTimeKind.Utc) : (DateTime?)null;
        }

        #endregion


        public override bool Equals(object obj)
        {
            if (!(obj is TimeOfDay))
            {
                return false;
            }

            TimeOfDay objTod = (TimeOfDay)obj;

            return objTod.Hour == Hour && objTod.Minute == Minute && objTod.Second == Second;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator==(TimeOfDay left, TimeOfDay right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(TimeOfDay left, TimeOfDay right)
        {
            return !(left == right);
        }

        public static bool operator< (TimeOfDay left, TimeOfDay right)
        {
            return left.Hour * 3600 + left.Minute * 60 + left.Second < right.Hour * 3600 + right.Minute * 60 + right.Second;
              
        }

        public static bool operator> (TimeOfDay left, TimeOfDay right)
        {
            return left.Hour * 3600 + left.Minute * 60 + left.Second > right.Hour * 3600 + right.Minute * 60 + right.Second;

        }

        public override string ToString()
        {
            return string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", Hour, Minute, Second);
        }

    }
}
