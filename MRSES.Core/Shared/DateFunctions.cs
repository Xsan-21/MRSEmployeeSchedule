using System;
using NodaTime;
using NodaTime.Text;
using System.Collections.Generic;
using System.Linq;

namespace MRSES.Core.Shared
{
    public struct DateFunctions
    {
        public static string ApplyCultureToLocalDate(LocalDate date, string culture)
        {
            throw new NotImplementedException();
        }

        public static DateTime FromDateTimeStringToDateTime(string date)
        {
            return DateTime.Parse(date);
        }

        public static LocalDate FromDateTimeStringToLocalDate(string date)
        {
            return LocalDatePattern.CreateWithInvariantCulture("dddd, MMMM d, yyyy").Parse(date).Value;
        }

        public static LocalDate FromDateTimeToLocalDate(DateTime dateTime)
        {
            return new LocalDate(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static LocalDate FromLocalDateStringToLocalDate(string localDate)
        {
            return LocalDatePattern.CreateWithInvariantCulture("dddd, MMMM d, yyyy").Parse(localDate).Value;
        }

        static public DateTime FromLocalTimeToDateTime(LocalTime time)
        {
            return new DateTime(2015, 1, 1, time.Hour, time.Minute, time.Second);
        }

        static public DateTime FromLocalDateToDateTime(LocalDate date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static List<string> DaysOfWeekInString(NodaTime.LocalDate ofWeek, System.Globalization.CultureInfo culture)
        {
            return GetWeekDays(ofWeek)
                .Select(day => day.ToString("dddd d", culture))
                .ToList();
        }

        public static IEnumerable<LocalDate> GetCurrentAndNextThreeWeeksFrom(LocalDate fromWeek)
        {
            for (int i = 0; i < 4; i++)
            {
                yield return fromWeek.PlusWeeks(i);
            }
        }

        public static IEnumerable<LocalDate> GetCurrentAndNextThreeWeeks()
        {
            for (int i = 0; i < 4; i++)
            {
                yield return CurrentWeek().PlusWeeks(i);
            }
        }

        public static IEnumerable<LocalDate> GetPreviousAndNextThreeWeeks()
        {
            var previousWeek = CurrentWeek().PlusWeeks(-1);
            for (int i = 0; i < 4; i++)
            {
                yield return previousWeek.PlusWeeks(i);
            }
        }

        public static IEnumerable<LocalDate> GetWeekDays(LocalDate week)
        {
            for (int i = 0; i <= 6; i++)
            {
                yield return week.PlusDays(i);
            }
        }

        public static LocalDate CurrentWeek()
        {
            IsoDayOfWeek firstWeekDay = GetStartWeek();
            LocalDate currentDate = GetCurrentDate.Date;

            if (currentDate.IsoDayOfWeek == firstWeekDay)
                return currentDate;

            return currentDate.Previous(firstWeekDay);
        }

        private static IsoDayOfWeek GetStartWeek()
        {
            IsoDayOfWeek dayOfWeek = IsoDayOfWeek.None;

            switch (Configuration.FirstDayOfWeek)
            {
                case "wednesday":
                    dayOfWeek = IsoDayOfWeek.Wednesday;
                    break;
                case "thursday":
                    dayOfWeek = IsoDayOfWeek.Thursday;
                    break;
                case "friday":
                    dayOfWeek = IsoDayOfWeek.Friday;
                    break;
                case "saturday":
                    dayOfWeek = IsoDayOfWeek.Saturday;
                    break;
                case "sunday":
                    dayOfWeek = IsoDayOfWeek.Sunday;
                    break;
                case "monday":
                    dayOfWeek = IsoDayOfWeek.Monday;
                    break;
                case "tuesday":
                    dayOfWeek = IsoDayOfWeek.Tuesday;
                    break;
                default:
                    dayOfWeek = IsoDayOfWeek.Monday;
                    break;
            }

            return dayOfWeek;
        }

        static public ZonedDateTime GetCurrentDate
        {
            get { return new ZonedDateTime(SystemClock.Instance.Now, DateTimeZoneProviders.Tzdb["America/Puerto_Rico"]); }
        }

        static public LocalTime FromDateTimeToLocalTime(DateTime dateTime)
        {
            return new LocalTime(dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}
