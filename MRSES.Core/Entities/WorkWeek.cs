using System;
using System.Collections.Generic;
using NodaTime;

namespace MRSES.Core.Entities
{
    public struct WorkWeek
    {
        public static IEnumerable<LocalDate> GetNextFourWeeksFrom(LocalDate fromWeek)
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
    }
}
