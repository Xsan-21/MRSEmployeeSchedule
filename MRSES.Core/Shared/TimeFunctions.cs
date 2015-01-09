using NodaTime;
using NodaTime.Text;
using System.Linq;

namespace MRSES.Core.Shared
{
    public struct TimeFunctions
    {
        #region Variables

        static System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(Configuration.CultureInfo);

        #endregion

        /// <summary>
        /// Verify if the turn format is correct. Input format should be like 8:00am - 5:00pm
        /// </summary>
        /// <param name="turn"></param>
        /// <returns></returns>
        static public bool FormatOfTurnIsValid(string turn)
        {
            bool result = false;

            if (turn.Contains("-"))
            {
                var turnInAndOut = SeparateHourInAndHourOut(turn);
                result = HourIsInLongFormat(turnInAndOut[0]) && HourIsInLongFormat(turnInAndOut[1]);
            }

            return result;
        }

        static public bool TryChangeShortHourFormatToLongFormat(System.Windows.Forms.TextBox textBox)
        {
            bool result = false;

            if (ContainsTermsToIgnore(textBox.Text, textBox.Tag.ToString()))
                return result;

            if (textBox.Text.Contains("-"))
            {
                string[] _result = new string[2];

                var getTurnInAndOut = SeparateHourInAndHourOut(textBox.Text);

                for (int i = 0; i < 2; i++)
                {
                    if (HourIsInShortFormat(getTurnInAndOut[i]))
                        _result[i] = ConvertHourInShortFormatToLong(getTurnInAndOut[i]);
                    else
                        _result[i] = getTurnInAndOut[i];
                }

                textBox.Text = _result[0] + " - " + _result[1];
                
                if(string.IsNullOrWhiteSpace(_result[1]))
                    textBox.Select(textBox.Text.Length, 0);

                result = true;
            }

            return result;
        }

        static public double TotalTurnHours(string firstTurn, string secondTurn)
        {
            return SumTurnHours(firstTurn) + SumTurnHours(secondTurn);
        }

        static public LocalTime[] GetTurnInAndOut(string turn)
        {
            if (!FormatOfTurnIsValid(turn))
                throw new System.Exception("El formato del turno no es válido.");

            string[] turnResult = SeparateHourInAndHourOut(turn);

            LocalTime turnIn = ConvertToLocalTime(turnResult[0]),
                      turnOut = ConvertToLocalTime(turnResult[1]);

            return new LocalTime[] { turnIn, turnOut };
        }

        static public LocalTime ConvertToLocalTime(string hour)
        {
            if (!HourIsInLongFormat(hour))
                throw new System.Exception("El formato de la hora no es válido. Por ejemplo, el formato de ser como 12:00pm");

            return LocalTimePattern.CreateWithInvariantCulture("h:mmtt").Parse(hour).Value;
        }

        static string ApplyCultureToLocalTime(LocalTime hour)
        {
            return hour.ToString("h:mmtt", culture);
        }

        static bool HourIsInLongFormat(string turnHour)
        {
            return LocalTimePattern.CreateWithInvariantCulture("h:mmtt").Parse(turnHour).Success;
        }

        static bool HourIsInShortFormat(string turnHour)
        {
            return HourFormatAndPatternMatches("ht", turnHour) || HourFormatAndPatternMatches("h.mt", turnHour);
        }

        static bool HourFormatAndPatternMatches(string patternText, string shortHour)
        {
            return LocalTimePattern.CreateWithInvariantCulture(patternText).Parse(shortHour).Success;
        }

        static string[] SeparateHourInAndHourOut(string turn)
        {
            if (!turn.Contains("-"))
                throw new System.Exception("");
            
            return turn.Replace(" ", "").Split('-');
        }

        //static public bool ValidateTotalHour(string hour)
        //{
        //    return System.Text.RegularExpressions.Regex.IsMatch(hour, @"^\d+(\.\d{2})?$");
        //}

        static string ConvertHourInShortFormatToLong(string shortHour)
        {
            if (!HourIsInShortFormat(shortHour))
                throw new System.Exception("La hora no está en forma corta. Ejemplo, de ser como 1p o 1.3p");

            if (HourFormatAndPatternMatches("ht", shortHour))
                return ConvertHourInShortFormatToLongWithSpecificPattern("ht", shortHour);
            else if (HourFormatAndPatternMatches("h.mt", shortHour))
                return ConvertHourInShortFormatToLongWithSpecificPattern("h.mt", shortHour);

            return shortHour;
        }

        /// <summary>
        /// Call FromShortToLongHour instead of this method.
        /// </summary>
        /// <param name="patternText"></param>
        /// <param name="shortHour"></param>
        /// <returns></returns>
        static string ConvertHourInShortFormatToLongWithSpecificPattern(string patternText, string shortHour)
        {
            return LocalTimePattern.CreateWithInvariantCulture(patternText).Parse(shortHour).Value.ToString("h:mmtt", culture);
        }

        static bool ContainsTermsToIgnore(string input, string otherTerm = "")
        {
            bool result = false;

            string[] termsToIgnore = 
            { 
                "X", "available", "nd", "no disponible", "disponible", otherTerm, string.Empty, null 
            };

            if (termsToIgnore.Any(term => term == input))
                result = true;

            return result;
        } 

        static double SumTurnHours(string turn)
        {
            if (!FormatOfTurnIsValid(turn))
                return 0;

            var turnInAndOut = GetTurnInAndOut(turn);
            return Period.Between(turnInAndOut[0], turnInAndOut[1]).ToDuration().ToTimeSpan().TotalHours;
        }
    }
}
