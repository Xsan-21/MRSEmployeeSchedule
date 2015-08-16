using System;
using NodaTime;
using NodaTime.Text;
using System.Collections.Generic;
using System.Linq;

namespace MRSES.Core.Shared
{
    public struct DateFunctions
    {
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

        public static List<string> DaysOfWeekInString(LocalDate ofWeek, System.Globalization.CultureInfo culture)
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
            if (StringFunctions.StringIsNullOrEmpty(Configuration.FirstDayOfWeek))
                throw new Exception("No se ha establecido día en que comienza la jornada de trabajo");

            IsoDayOfWeek firstWeekDay = FirstDayOfWeek(Configuration.FirstDayOfWeek);
            LocalDate currentDate = GetCurrentDate.Date;

            if (currentDate.IsoDayOfWeek == firstWeekDay)
                return currentDate;

            return currentDate.Previous(firstWeekDay);
        }

        public static LocalDate GetWeekOf(LocalDate date)
        {
            if (StringFunctions.StringIsNullOrEmpty(Configuration.FirstDayOfWeek))
                throw new Exception("No se ha establecido día en que comienza la jornada de trabajo");

            var firstWeekDay = FirstDayOfWeek(Configuration.FirstDayOfWeek);
            
            if (date.IsoDayOfWeek != firstWeekDay)
            {
                for (int i = 1; i < 7; i++)
                {
                    if (date.PlusDays(-i).IsoDayOfWeek == firstWeekDay)
                    {
                        return date.PlusDays(-i);
                    }
                }
            }

            return date;
        }

        public static IsoDayOfWeek FirstDayOfWeek(string day)
        {
            IsoDayOfWeek dayOfWeek = IsoDayOfWeek.None;

            switch (day)
            {
                case "Wednesday":
                    dayOfWeek = IsoDayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    dayOfWeek = IsoDayOfWeek.Thursday;
                    break;
                case "Friday":
                    dayOfWeek = IsoDayOfWeek.Friday;
                    break;
                case "Saturday":
                    dayOfWeek = IsoDayOfWeek.Saturday;
                    break;
                case "Sunday":
                    dayOfWeek = IsoDayOfWeek.Sunday;
                    break;
                case "Monday":
                    dayOfWeek = IsoDayOfWeek.Monday;
                    break;
                case "Tuesday":
                    dayOfWeek = IsoDayOfWeek.Tuesday;
                    break;
                case "miércoles":
                    dayOfWeek = IsoDayOfWeek.Wednesday;
                    break;
                case "jueves":
                    dayOfWeek = IsoDayOfWeek.Thursday;
                    break;
                case "viernes":
                    dayOfWeek = IsoDayOfWeek.Friday;
                    break;
                case "sábado":
                    dayOfWeek = IsoDayOfWeek.Saturday;
                    break;
                case "domingo":
                    dayOfWeek = IsoDayOfWeek.Sunday;
                    break;
                case "lunes":
                    dayOfWeek = IsoDayOfWeek.Monday;
                    break;
                    case "martes":
                    dayOfWeek = IsoDayOfWeek.Tuesday;
                    break;
                default:
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
