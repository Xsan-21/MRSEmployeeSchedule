using System;
using NodaTime;
using NodaTime.Text;

namespace MRSES.Core.Shared
{
    public struct DateFunctions
    {
        public static DateTime FromDateTimeStringToDateTime(string date)
        {
            return DateTime.Parse(date);
        }

        public static DateTime FromLocalDateToDateTime(LocalDate localDate)
        {
            return new DateTime(localDate.Year, localDate.Month, localDate.Day);
        }

        public static LocalDate FromDateTimeToLocalDate(DateTime dateTime)
        {
            return new LocalDate(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static LocalDate FromShortDateTimeFormatToLocalDate(string shortDateTime)
        {
            return LocalDatePattern.CreateWithInvariantCulture("M/d/yyyy").Parse(shortDateTime).Value;
        }

        public static LocalDate FromLocalDateStringToLocalDate(string localDate)
        {
            return LocalDatePattern.CreateWithInvariantCulture("dddd, MMMM d, yyyy").Parse(localDate).Value;
        }

        public static LocalDateTime ConvertDateTimeToLocalDateTime(DateTime date, LocalTime time)
        {
            return new LocalDateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute);
        }

        public static DateTime ConvertLocalDateTimeToDateTime(LocalDateTime localDateTime)
        {
            return localDateTime.ToDateTimeUnspecified();
        }

        public static LocalDateTime FromDateTimeToLocalDateTime(DateTime petitionDate)
        {
            return new LocalDateTime(petitionDate.Year, petitionDate.Month, petitionDate.Day, petitionDate.Hour, petitionDate.Minute, petitionDate.Second);
        }
    }
}
