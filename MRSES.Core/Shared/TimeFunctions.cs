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

        static public bool TurnHourIsInLongFormat(string turnHour)
        {
            return System.Text.RegularExpressions.Regex.Match(turnHour, @"(1[012]|[1-9]):[0-5][0-9](\\s)?(?i)(am|pm)").Success;
           
        }

        static public bool TurnHourIsInShortFormat(string turnHour)
        {
            if (turnHour.IndexOfAny(new[] { 'a', 'p' }) < 0)
                throw new System.Exception("La hora especificada no indica si es de día o de tarde. Por favor indique a o p en la hora.");

            return SpecifiedPatternAndHourAreValid("ht", turnHour) || SpecifiedPatternAndHourAreValid("h.mt", turnHour);
        }

        static string[] SeparateHourInAndHourOut(string turn)
        {
            return turn.Replace(" ","").Split('-');
        }

        //static public bool ValidateTotalHour(string hour)
        //{
        //    return System.Text.RegularExpressions.Regex.IsMatch(hour, @"^\d+(\.\d{2})?$");
        //}

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

        static bool ValidateTurnHourFormat(string turn)
        {
            return TurnHourIsInShortFormat(turn) || TurnHourIsInLongFormat(turn);
        }

        static public void ChangeShortFormatToLongFormat(System.Windows.Forms.TextBox textBox)
        {
            var turn = textBox.Text.Trim();

            var termsToIgnore = new[] {textBox.Tag.ToString(), "X", "available", "nd", "no disponible", "disponible", string.Empty, null };

            for (int i = 0; i < termsToIgnore.Length; i++)
            {
                if (turn == termsToIgnore[i])
                    return;
            }

            string[] result = new string[2];

            if (!turn.Contains("-"))
                throw new System.Exception("Por favor indique hora de entrada y salida separada por un guión (Ej. 11:00am - 2:00pm o 11a-2p).");

            var getTurnInAndOut = SeparateHourInAndHourOut(turn);

            for (int i = 0; i < 2; i++)
			{
			    if (ValidateTurnHourFormat(getTurnInAndOut[i]))
                {
                    if (TurnHourIsInShortFormat(getTurnInAndOut[i]))
                        result[i] = FromShortToLongHour(getTurnInAndOut[i]);
                    else
                        result[i] = getTurnInAndOut[i];
                }
                else
                {
                    throw new System.Exception("La hora especificada no tiene un formato correcto.");
                }
			}

            textBox.Text = result[0] + " - " + result[1];
        }
    }
}
