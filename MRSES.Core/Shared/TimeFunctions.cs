using NodaTime;
using NodaTime.Text;

namespace MRSES.Core.Shared
{
    public struct TimeFunctions
    {
        static public LocalTime[] GetTurnInAndOut(string turn)
        {
            if (turn.Contains("X") || string.IsNullOrEmpty(turn))
                return new LocalTime[] {new LocalTime(), new LocalTime() };

            string[] turnResult = SeparateHourInAndHourOut(turn);

            LocalTime turnIn = ConvertToLocalTime(turnResult[0]),
                      turnOut = ConvertToLocalTime(turnResult[1]);

            return new LocalTime[] { turnIn, turnOut };
        }

        static public LocalTime ParseLocalTimeFromString(string time)
        {
            return LocalTimePattern.CreateWithInvariantCulture("h:mmtt").Parse(time).Value;
        }

        static public LocalTime ConvertToLocalTime(string hour)
        {
            return LocalTimePattern.CreateWithInvariantCulture("h:mmtt").Parse(hour).Value;
        }

        static public bool TurnHourIsInLongFormat(string turn)
        {
            bool success = true;

            if (!turn.Contains("-"))
                throw new System.Exception("La hora especificada esta incompleta. Por favor indique hora de entrada y salida separada por un guión (Ej. 11:00am - 2:00pm).");

            var result = SeparateHourInAndHourOut(turn);

            foreach (var element in result)
                if (element != "" && success)
                    success = ValidateHourIn12HourFormat(element);
    
            return success;
        }

        static public bool TurnHourIsInShortFormat(string shortHour)
        {
            BasicRequirementsForShortTimeFormat(shortHour);

            var turnInAndOut = SeparateHourInAndHourOut(shortHour);

            var turnIn = turnInAndOut[0];
            var turnOut = turnInAndOut[1];

            var turnInIsValid = SpecifiedPatternAndHourAreValid("ht", turnIn) || SpecifiedPatternAndHourAreValid("h.mt", turnIn);
            var turnOutIsValid = SpecifiedPatternAndHourAreValid("ht", turnOut) || SpecifiedPatternAndHourAreValid("h.mt", turnOut);

            return turnInIsValid && turnOutIsValid;
        }

        static void BasicRequirementsForShortTimeFormat(string turn)
        {
            if (!turn.Contains("-"))
                throw new System.Exception("La hora especificada esta incompleta. Por favor indique hora de entrada y salida separada por un guión (Ej. 11a - 2p).");
            
            var getTurnInAndOut = SeparateHourInAndHourOut(turn);

            foreach (var hour in getTurnInAndOut)
		        if(hour.IndexOfAny(new[]{'a','p'}) < 0)
                    throw new System.Exception("La hora especificada no indica si es de día o de tarde. Por favor indique a o p en la hora.");            
        }

        static bool ValidateHourIn12HourFormat(string hour)
        {
            return System.Text.RegularExpressions.Regex.Match(hour, @"(1[012]|[1-9]):[0-5][0-9](\\s)?(?i)(am|pm)").Success;
        }

        static string[] SeparateHourInAndHourOut(string text)
        {
            return text.Replace(" ","").Split('-');
        }

        static public bool ValidateTotalHour(string hour)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(hour, @"^\d+(\.\d{2})?$");
        }

        static string FromShortToLongHour(string shortHour)
        {
            string result = shortHour;

            if (SpecifiedPatternAndHourAreValid("ht", shortHour))
                result = ConvertShortHourToLongWithSpecificPattern("ht", shortHour);
            else if (SpecifiedPatternAndHourAreValid("h.mt", shortHour))
                result = ConvertShortHourToLongWithSpecificPattern("h.mt", shortHour);

            return result;
        }

        /// <summary>
        /// Call FromShortToLongHour instead of this method.
        /// </summary>
        /// <param name="patternText"></param>
        /// <param name="shortHour"></param>
        /// <returns></returns>
        static string ConvertShortHourToLongWithSpecificPattern(string patternText, string shortHour)
        {
            return LocalTimePattern.CreateWithInvariantCulture(patternText).Parse(shortHour).Value.ToString("h:mmtt", new System.Globalization.CultureInfo("en-US"));
        }

        static bool SpecifiedPatternAndHourAreValid(string patternText, string shortHour)
        {
            return LocalTimePattern.CreateWithInvariantCulture(patternText).Parse(shortHour).Success;
        }

        static public bool ValidateTurnHourFormat(string turn)
        {
            return TurnHourIsInShortFormat(turn) || TurnHourIsInLongFormat(turn);
        }

        static public void ChangeShortFormatToLongFormat(System.Windows.Forms.TextBox textBox)
        {
            var turn = textBox.Text;

            if (turn != textBox.Tag.ToString() || !string.IsNullOrEmpty(turn) || !turn.Contains("X"))
            {
                if (TurnHourIsInShortFormat(turn))
                {
                    textBox.Text = FromShortToLongHour(turn);
                }
                else if(TurnHourIsInLongFormat(turn))
                {
                    return;
                }
                else
                {
                    throw new System.Exception("La hora especificada no tiene un formato correcto.");
                }
            }
        }
    }
}
