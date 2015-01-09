using System;
using System.Collections.Generic;
using NodaTime;

namespace MRSES.Core.Entities
{
    public struct WorkWeek
    {
        static System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(Configuration.CultureInfo);

        public static IEnumerable<LocalDate> GetCurrentAndNextThreeWeeksFrom(LocalDate fromWeek)
        {
            for (int i = 0; i < 4; i++)
            {
                yield return fromWeek.PlusWeeks(i);
            }
        }

        public static IEnumerable<LocalDate> GetNextFourWeeksFromCurrentWeek()
        {
            for (int i = 0; i < 4; i++)
            {
                yield return CurrentWeek().PlusWeeks(i);
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
            LocalDate currentDate = new LocalDate(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

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
    }
}
