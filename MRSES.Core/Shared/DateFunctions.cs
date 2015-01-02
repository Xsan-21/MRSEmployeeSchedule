using System;
using NodaTime;
using NodaTime.Text;

namespace MRSES.Core.Shared
{
    public struct DateFunctions
    {
        #region Variables

        static System.Globalization.CultureInfo culture_usa = new System.Globalization.CultureInfo("en-US");
        static System.Globalization.CultureInfo culture_pr = new System.Globalization.CultureInfo("es-PR"); // .ToString("dd \\de MMMM \\de yyyy", new System.Globalization.CultureInfo("es-PR")

        #endregion

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

        //public static LocalDate FromShortDateTimeFormatToLocalDate(string shortDateTime)
        //{
        //    return LocalDatePattern.CreateWithInvariantCulture("M/d/yyyy").Parse(shortDateTime).Value;
        //}

        //public static LocalDate FromLocalDateStringToLocalDate(string localDate)
        //{
        //    return LocalDatePattern.CreateWithInvariantCulture("dddd, MMMM d, yyyy").Parse(localDate).Value;
        //}

        static public DateTime FromLocalTimeToDateTime(LocalTime time)
        {
            return new DateTime(2015, 1, 1, time.Hour, time.Minute, time.Second);
        }

        static public DateTime FromLocalDateToDateTime(LocalDate date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
    }
}
