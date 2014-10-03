using NodaTime;
using NodaTime.Text;

namespace MRSES.Core.Shared
{
    public struct TimeFunctions
    {
        public static LocalTime[] GetTurnInAndOut(string turn)
        {
            if (turn.Contains("X") || turn == "" || turn == null)
                return new LocalTime[] {new LocalTime(), new LocalTime() };

            string[] turnResult = turn.Replace(" ", "").Split('-');

            LocalTime turnIn = ConvertToLocalTime(turnResult[0]),
                      turnOut = ConvertToLocalTime(turnResult[1]);

            return new LocalTime[] { turnIn, turnOut };
        }

        public static LocalTime ParseLocalTimeFromString(string time)
        {
            return LocalTimePattern.CreateWithInvariantCulture("h:mmtt").Parse(time).Value;
        }

        public static LocalTime ConvertToLocalTime(string hour)
        {
            return LocalTimePattern.CreateWithInvariantCulture("h:mmtt").Parse(hour).Value;
        }

        public static bool ValidateTimeRangeFormat(string text)
        {
            bool success = false;

            if (text.Contains("-"))
            {
                var result = SeparateTwoHours(RemoveWhiteSpaceFromString(text));

                foreach (var element in result)
                    if (element != "")
                        success = ValidateHourIn12HourFormat(element);
            }

            return success;
        }

        public static bool ValidateHourIn12HourFormat(string hour)
        {
            return System.Text.RegularExpressions.Regex.Match(hour, @"(1[012]|[1-9]):[0-5][0-9](\\s)?(?i)(am|pm)").Success;
        }

        public static string RemoveWhiteSpaceFromString(string text)
        {
            return text.Replace(" ", "");
        }

        public static string[] SeparateTwoHours(string text)
        {
            return text.Split('-');
        }

        public static bool ValidateTotalHour(string hour)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(hour, @"^\d+(\.\d{2})?$");
        }
    }
}
