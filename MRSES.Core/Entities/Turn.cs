using System;
using System.Globalization;
using NodaTime;


namespace MRSES.Core.Entities
{
    public interface ITurn
    {
        LocalDate OfWeek { get; set; }
        LocalDate Date { get; set; } 
        string FirstTurn { get; }
        string SecondTurn { get; }
        LocalTime TurnIn1 { get; set; }
        LocalTime TurnOut1 { get; set; }
        LocalTime TurnIn2 { get; set; }
        LocalTime TurnOut2 { get; set; }
        double Hours { get; }
        bool IsFreeDay { get; }
    }

    public class Turn : ITurn
    {
        #region VARIABLES AND PROPERTIES
        
        CultureInfo culture;

        public LocalDate OfWeek { get; set; }

        LocalDate _date;
        public LocalDate Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public string FirstTurn
        {
            get
            {
                return ToString(TurnIn1, TurnOut1);
            }
        }

        public string SecondTurn
        {
            get
            {
                return ToString(TurnIn2, TurnOut2);
            }
        }

        LocalTime _turnIn1;
        public LocalTime TurnIn1
        {
            get { return _turnIn1; }
            set { _turnIn1 = value; }
        }

        LocalTime _turnOut1;
        public LocalTime TurnOut1
        {
            get { return _turnOut1; }
            set { _turnOut1 = value; }
        }

        LocalTime _turnIn2;
        public LocalTime TurnIn2
        {
            get { return _turnIn2; }
            set { _turnIn2 = value; }
        }

        LocalTime _turnOut2;
        public LocalTime TurnOut2
        {
            get { return _turnOut2; }
            set { _turnOut2 = value; }
        }

        public double Hours
        {
            get { return GetTotalHoursForThisTurn(); }
        }

        public bool IsFreeDay
        {
            get { return Hours == 0; }
        }

        #endregion VARIABLES AND PROPERTIES

        #region CONTRUCTORS

        public Turn(LocalDate ofWeek, LocalDate date, LocalTime turnIn1, LocalTime turnOut1, LocalTime turnIn2, LocalTime turnOut2)
        {
            OfWeek = ofWeek;
            Date = date;
            TurnIn1 = turnIn1;
            TurnOut1 = turnOut1;
            TurnIn2 = turnIn2;
            TurnOut2 = turnOut2;
        }

        public Turn(LocalDate ofWeek, LocalTime turnIn1, LocalTime turnOut1, LocalTime turnIn2, LocalTime turnOut2)
        {
            OfWeek = ofWeek;
            TurnIn1 = turnIn1;
            TurnOut1 = turnOut1;
            TurnIn2 = turnIn2;
            TurnOut2 = turnOut2;
        }

        public Turn(LocalDate ofWeek, LocalDate date, LocalTime turnIn1, LocalTime turnOut1)
        {
            OfWeek = ofWeek;
            Date = date;
            TurnIn1 = turnIn1;
            TurnOut1 = turnOut1;
        }

        public Turn(LocalDate ofWeek, LocalTime turnIn1, LocalTime turnOut1)
        {
            OfWeek = ofWeek;
            TurnIn1 = turnIn1;
            TurnOut1 = turnOut1;
        }

        public Turn(LocalDate ofWeek, LocalDate date)
        {
            OfWeek = ofWeek;
            Date = date;
        }

        public Turn()
        {
            culture = new CultureInfo("en-US");
            Date = new ZonedDateTime(SystemClock.Instance.Now, DateTimeZoneProviders.Tzdb["America/Puerto_Rico"]).Date;
        }

        #endregion

        #region METHODS

        double GetTotalHoursForThisTurn()
        {
            double result = CalculateHours(TurnIn1, TurnOut1) + CalculateHours(TurnIn2, TurnOut2);

            if (result < 0)
                throw new Exception("Uno de los turnos asignados está incorrecto");

            return result;
        }

        double CalculateHours(LocalTime turnIn, LocalTime turnOut)
        {
            return Period.Between(turnIn, turnOut).ToDuration().ToTimeSpan().TotalHours;
        }

        string ToString(LocalTime turnIn, LocalTime turnOut)
        {
            if (CalculateHours(turnIn, turnOut) == 0)
                return "XXXXXX";

            return string.Format("{0}-{1}", turnIn.ToString("h:mmtt", culture), turnOut.ToString("h:mmtt", culture));
        }

        #endregion METHODS

        #region HELPERS

        public static Turn Create(LocalDate ofWeek, LocalDate date)
        {
            return new Turn(ofWeek, date);
        }

        public static Turn Create(LocalDate ofWeek, LocalDate date, LocalTime turnIn1, LocalTime turnOut1)
        {
            return new Turn(ofWeek, date, turnIn1, turnOut1);
        }

        public static Turn Create(LocalDate ofWeek, LocalDate date, LocalTime turnIn1, LocalTime turnOut1, LocalTime turnIn2, LocalTime turnOut2)
        {
            return new Turn(ofWeek, date, turnIn1, turnOut1, turnIn2, turnOut2);
        }

        #endregion
    }
}
