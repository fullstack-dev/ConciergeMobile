using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Home-grown class for representing a specific day. Made to be JSON/REST-friendly
    /// Note that this class is typically "time zone agnostic" in that it is for logic that 
    /// does not depend a specific moment in time, but is for general times.
    /// For a specific moment in time, use the <see cref="TimeStamp"/> class.
    /// </summary>  
    public struct SimpleDate : IUrlFormatable, IComparable
    {
        public int Year
        {
            get;
            /*private*/ set;
        }

        public int Month
        {
            get;
            /*private*/ set;
        }

        public int Day
        {
            get;
            /*private*/ set;
        }

        //public Date()
        //{

        //}

        public SimpleDate(int year, int month, int day)
        {
            Year = year;
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("month", "Months can only be from 1-12");
            }
            Month = month;
            if (day < 1)
            {
                throw new ArgumentOutOfRangeException("day", "Day must be at least 1");

            }
            int upperDayLimit;
            switch (month)
            {
                case 2:
                {
                    upperDayLimit = IsLeapYear(year) ? 29 : 28;
                    break;
                }
                case 4:
                case 6:
                case 9:
                case 11:
                {
                    upperDayLimit = 30;
                    break;
                }
                default:
                {
                    upperDayLimit = 31;
                    break;
                }
            }
            if (day > upperDayLimit)
            {
                throw new ArgumentOutOfRangeException("day", $"Day must be no more than {upperDayLimit}");
            }
            Day = day;
        }

        public override string ToString()
        {
            if (Year != 0)
            {
                return ((DateTime)this).ToString("d");
            }
            else
            {
                return $"Invalid: {Month}/{Day}/{Year}";
            }
        }

        public string GetUrlSafeString()
        {
            return $"{Year}_{Month}_{Day}";
        }

        public SimpleDate AddDays(int daysToAdd)
        {
            int days = ToDays() + daysToAdd;

            return FromDays(days);

        }

        public SimpleDate AddMonths(int monthsToAdd)
        {
            int year, month;
            year = Year + monthsToAdd / 12;
            month = Month + (monthsToAdd % 12);

            if (month > 12)
            {
                year++;
                month -= 12;
            }
            else if (month < 1)
            {
                year--;
                month += 12;

            }

            return new SimpleDate(year, month, Day);
        }

        public static bool TryParse(string source, out SimpleDate result)
        {
            try
            {
                result = Parse(source);
                return true;
            }
            catch
            {
                result = default(SimpleDate);
                return false;
            }

        }

        public static SimpleDate Parse(string source)
        {
            var parts = source.Split('_');
            if (parts.Length == 3)
            {
                SimpleDate result = new SimpleDate(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                return result;
            }
            else
            {
                DateTime parsedDT;
                if (DateTime.TryParse(source, out parsedDT))
                {
                    return (SimpleDate)parsedDT;
                }
            }
            throw new FormatException($"Could not parse '{source}' as a {typeof(SimpleDate)} object.");
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is SimpleDate)
            {
                var asType = (SimpleDate)obj;
                return Year == asType.Year && Month == asType.Month && Day == asType.Day;
            }
            return false;
        }

        

        public override int GetHashCode()
        {
            return Year.GetHashCode() + (Month.GetHashCode() << 1) + (Day.GetHashCode() << 2);
        }

        public int CompareTo(object obj)
        {
            SimpleDate other = (SimpleDate)obj;
            if (this < other)
            {
                return -1;
            }
            else if (this > other)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static bool operator < (SimpleDate left, SimpleDate right)
        {
            if (left.Year == right.Year)
            {
                if (left.Month == right.Month)
                {
                    return left.Day < right.Day;
                }
                else
                {
                    return left.Month < right.Month;
                }
            }
            else
            {
                return left.Year < right.Year;
            }
        }


        public static bool operator == (SimpleDate left, SimpleDate right)
        {
            return left.Equals(right);
        }


        public static bool operator != (SimpleDate left, SimpleDate right)
        {
            return !left.Equals(right);
        }

        public static bool operator > (SimpleDate left, SimpleDate right)
        {
            if (left.Year == right.Year)
            {
                if (left.Month == right.Month)
                {
                    return left.Day > right.Day;
                }
                else
                {
                    return left.Month > right.Month;
                }
            }
            else
            {
                return left.Year > right.Year;
            }

        }

        /// <summary>
        /// Converts this date to a built-in <see cref="DateTime"/> class.
        /// Use caution with this conversion! The resulting DateTime object has a meaningless time inside it!
        /// </summary>
        /// <param name="source"></param>
        public static explicit operator DateTime(SimpleDate source)
        {
            return new DateTime(source.Year, source.Month, source.Day, 0, 0, 0, 0, DateTimeKind.Unspecified);
        }

        public static explicit operator SimpleDate(DateTime source)
        {
            if (source.Kind != DateTimeKind.Utc)
            {
                return new SimpleDate(source.Year, source.Month, source.Day);
            }
            else
            {
                throw new InvalidCastException("Invalid cast, you can only cast 'unspecified' DateTime types. Make sure your DateTime is not time-zone specific.");
            }
        }

        private const int EpochYear = 2000;
        private const int EpochMonth = 1;
        private const int EpochDay = 1;

        public int ToDays()
        {
            int dayTotal = 0;

            //add base years
            if (Year > EpochYear)
            {
                dayTotal += (Year - EpochYear - 1) * 365;
            }
            //adjust for leap years for whole years on the range
            dayTotal += NumberOfLeapYears(EpochYear + 1, Year - 1);
            //add days so far this year prior to this month
            for (int i = 1; i < Month; i++)
            {
                dayTotal += DayCount(i, Year);
            }
            //add days of this month
            dayTotal += Day - 1;

            //now add days of epoch year.
            if (Year > EpochYear)
            {
                dayTotal += DaysLeftInYear(EpochYear, EpochMonth, EpochDay) + 1;
            }
            return dayTotal;
        }

        public static SimpleDate FromDays(int days)
        {
            int count = days;
            int year;
            int daysLeftInEpoch = DaysLeftInYear(EpochYear, EpochMonth, EpochDay);

            if (count > daysLeftInEpoch)
            {
                count -= (daysLeftInEpoch + 1);
                year = EpochYear + 1;
                int daysLeftInCurrentYear = DaysLeftInYear(year, 1, 1);
                while (count > daysLeftInCurrentYear)
                {
                    count -= (daysLeftInCurrentYear + 1);
                    year++;
                    daysLeftInCurrentYear = DaysLeftInYear(year, 1, 1);
                }
                SimpleDate result = FromDayOfYear(year, count);
                return result;
            }
            else
            {
                SimpleDate result = FromDayOfYear(EpochYear,  count - (DaysLeftInYear(EpochYear, 1, 1) - DaysLeftInYear(EpochYear, EpochDay, EpochDay)) );
                return result;
            }
        }

        private static SimpleDate FromDayOfYear(int year, int dayOfYear)
        {
            int count = dayOfYear;
            int month = 1;
            int daysLeftInMonth = DayCount(month, year);
            while (count >= daysLeftInMonth)
            {
                count -= daysLeftInMonth;
                month++;
                daysLeftInMonth = DayCount(month, year);
            }

            SimpleDate result = new SimpleDate(year, month, count + 1);

            return result;
        }

        private static int NumberOfLeapYears(int startYear, int endYear)
        {
            int count = 0;
            //very inefficient and primitive. but it works.
            for (int i = startYear; i<= endYear; i++)
            {
                if (IsLeapYear(i))
                {
                    count++;
                }
            }
            return count;
        }

        private static bool IsLeapYear(int year)
        {
            if (year % 4 == 0)
            {
                if (year % 100 == 0)
                {
                    if (year % 400 == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// returns months since epoch
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private static int MonthCount(int year, int month)
        {
            int count = 0;
            if (year > EpochYear)
            {
                count += 12 * (year - EpochYear - 1);
                count += 12 - EpochMonth;
                count += month - 1;
            }
            else
            {
                count += month - EpochMonth;
            }

            return count;
        }


        private static int DaysLeftInYear(int year, int month, int day)
        {
            int count = 0;
            for (int i = 12; i > month; i--)
            {
                count += DayCount(i, year);
            }
            count += DayCount(month, year) - day;

            return count;
        }

        /// <summary>
        /// returns the number of days in the specified month
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static int DayCount(int month, int year)
        {
            int count = 0;
            switch (month)
            {
                case 2:
                {
                    count = IsLeapYear(year) ? 29 : 28;
                    break;
                }
                case 4:
                case 6:
                case 9:
                case 11:
                {
                    count = 30;
                    break;
                }
                default:
                {
                    count = 31;
                    break;
                }
            }

            return count;

        }


    }
}
