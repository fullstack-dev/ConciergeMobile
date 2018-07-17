using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class to handle marshallng timestamps across REST API in a consistent way
    /// </summary>
    /// <remarks>
    /// JSON doesn't have an official type for "time". Different JSON implementations treat .Net time in different ways
    /// This is a class that allow times to be called across the REST API and not have to worry that the 
    /// server will be able to interpret it, the time will always be transmitted as an ISO GMT time.
    /// </remarks>
    public class TimeStamp : IComparable<TimeStamp>, IComparable<DateTime>
    {

        #region Private Data
        DateTime _internalRepresentation;
        #endregion

        /// <summary>
        /// Returns a 'universal time' string representation off the time.
        /// </summary>
        public string Time
        {
            get
            {
                return _internalRepresentation.ToUniversalTime().ToString("u");
            }
            set
            {
                _internalRepresentation = DateTime.Parse(value).ToUniversalTime();
            }
        }

        /// <summary>
        /// Converted this time to a string
        /// </summary>
        /// <returns>a string value representative of this time</returns>
        public override string ToString()
        {
            return Time;
        }

        /// <summary>
        /// value equality test
        /// Any object which has a string representation equivalent to this will be considered equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj != null && ToString().Equals(obj.ToString());// obj is TimeStamp && _internalRepresentation.Equals(((TimeStamp)obj)._internalRepresentation);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public static TimeStamp Now
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

        public static TimeStamp Parse(string timeStamp)
        {
            return new TimeStamp() { Time = timeStamp };
        }

        public static bool TryParse(string timeStamp, out TimeStamp value)
        {
            try
            {
                value = (TimeStamp)(DateTime)(TimeStamp.Parse(timeStamp));
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public int CompareTo(DateTime other)
        {
            return ((DateTime)this).CompareTo(other);
        }

        public int CompareTo(TimeStamp other)
        {
            return CompareTo((DateTime)other);
        }

        public static implicit operator TimeStamp(DateTime source)
        {
            TimeStamp newValue = new TimeStamp();
            if (source.Kind == DateTimeKind.Unspecified)
            {
                newValue._internalRepresentation = new DateTime(source.Ticks, DateTimeKind.Utc);
            }
            else
            {
                newValue._internalRepresentation = source.ToUniversalTime();
            }
            return newValue;
        }

        public static implicit operator DateTime(TimeStamp source)
        {
            return source != null ? source._internalRepresentation : default(DateTime);
        }

        public static implicit operator DateTime?(TimeStamp source)
        {
            return source != null ? (DateTime?)source._internalRepresentation : null;
        }

        public static bool operator <(TimeStamp lhs, TimeStamp rhs)
        {
            return ((DateTime)lhs) < ((DateTime)rhs);
        }

        public static bool operator >(TimeStamp lhs, TimeStamp rhs)
        {
            return ((DateTime)lhs) > ((DateTime)rhs);
        }

        public static bool operator <=(TimeStamp lhs, TimeStamp rhs)
        {
            return !(lhs > rhs);
        }

        public static bool operator >=(TimeStamp lhs, TimeStamp rhs)
        {
            return !(lhs < rhs);
        }
    }
}
