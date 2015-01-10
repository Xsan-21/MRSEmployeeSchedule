using System;
using NodaTime;


namespace MRSES.Core.Entities
{
    public interface ITurn
    {
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

        public LocalDate Date { get; set; }
        
        public LocalTime TurnIn1 { get; set; }

        public LocalTime TurnOut1 { get; set; }

        public LocalTime TurnIn2 { get; set; }

        public LocalTime TurnOut2 { get; set; }

        public double Hours
        {
            get { return GetTotalHoursForThisTurn(); }
        }

        public bool IsFreeDay
        {
            get { return Hours == 0; }
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

        #endregion VARIABLES AND PROPERTIES

        #region CONTRUCTORS

        public Turn(LocalDate date, LocalTime turnIn1, LocalTime turnOut1, LocalTime turnIn2, LocalTime turnOut2)
        {
            Date = date;
            TurnIn1 = turnIn1;
            TurnOut1 = turnOut1;
            TurnIn2 = turnIn2;
            TurnOut2 = turnOut2;
        }

        public Turn(LocalDate date, LocalTime turnIn1, LocalTime turnOut1)
        {
            Date = date;
            TurnIn1 = turnIn1;
            TurnOut1 = turnOut1;
        }

        public Turn(LocalDate date)
        {
            Date = date;
        }

        public Turn() : this(Core.Shared.DateFunctions.CurrentWeek())
        {
            
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
                return string.Empty;

            return string.Format("{0} - {1}", turnIn.ToString("h:mmtt", Configuration.CultureInfo), turnOut.ToString("h:mmtt", Configuration.CultureInfo));
        }

        #endregion METHODS

        #region HELPERS

        public static Turn Create(LocalDate date)
        {
            return new Turn(date);
        }

        public static Turn Create(LocalDate date, LocalTime turnIn1, LocalTime turnOut1)
        {
            return new Turn(date, turnIn1, turnOut1);
        }

        public static Turn Create(LocalDate date, LocalTime turnIn1, LocalTime turnOut1, LocalTime turnIn2, LocalTime turnOut2)
        {
            return new Turn(date, turnIn1, turnOut1, turnIn2, turnOut2);
        }

        #endregion
    }
}
