using System;
using NodaTime;


namespace MRSES.Core.Entities
{
    public interface ITurn
    {
        string ObjectId { get; set; }
        LocalDate Date { get; set; } 
        string FirstTurn { get; }
        string SecondTurn { get; }
        LocalTime[] TurnHours { get; set; }
        double Hours { get; }
        bool IsFreeDay { get; }
    }

    public struct Turn : ITurn
    {
        #region VARIABLES AND PROPERTIES

        public string ObjectId { get; set; }

        public LocalDate Date { get; set; }
        
        public LocalTime[] TurnHours { get; set; }

        public double Hours
        {
            get { return CalculateTurnHours(); }
        }

        public bool IsFreeDay
        {
            get { return Hours == 0; }
        }

        public string FirstTurn
        {
            get
            {
                var result = string.Format("{0} - {1}", ApplyFormat(TurnHours[0]), ApplyFormat(TurnHours[1]));
                return result == "12:00AM - 12:00AM" ? "" : result;
            }
        }

        public string SecondTurn
        {
            get
            {
                var result = string.Format("{0} - {1}", ApplyFormat(TurnHours[2]), ApplyFormat(TurnHours[3]));
                return result == "12:00AM - 12:00AM" ? "" : result;
            }
        }

        #endregion VARIABLES AND PROPERTIES

        #region CONTRUCTORS

        public Turn(LocalDate date, LocalTime[] turn) : this(date)
        {
            if (turn.Length > 4)
                throw new Exception("La cantidad de turnos es incorrecta. El máximo es 2 por día.");

            TurnHours = turn;
        }

        public Turn(LocalDate date)
        {
            ObjectId = string.Empty;
            Date = date;
            TurnHours = new LocalTime[4];
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Only for facilitate the database insertion in ScheduleRepository.
        /// </summary>
        /// <returns>Turns to be saved in database</returns>
        internal DateTime[] GetTurn()
        {
            var turns = new DateTime[4];

            for (int i = 0; i < turns.Length; i++)
            {
                turns[i] = Shared.DateFunctions.FromLocalTimeToDateTime(TurnHours[i]);
            }

            return turns;
        }

        string ApplyFormat(LocalTime time)
        {
            return time.ToString("h:mmtt", Configuration.CultureInfo);
        }

        double CalculateTurnHours()
        {
            return SumTurnHours(TurnHours[0], TurnHours[1]) + SumTurnHours(TurnHours[2], TurnHours[3]);
        }

        double SumTurnHours(LocalTime timeIn, LocalTime timeOut)
        {
            return Period.Between(timeIn, timeOut).ToDuration().ToTimeSpan().TotalHours;
        }

        public override string ToString()
        {
            string result = "";

            if (FirstTurn != "" && SecondTurn != "")
                result = FirstTurn + " | " + SecondTurn;
            else if (FirstTurn != "")
                result = FirstTurn;
            else if (SecondTurn != "")
                result = SecondTurn;

            return result;
        }

        #endregion METHODS

        #region HELPERS

        public static Turn Create(LocalDate date)
        {
            return new Turn(date);
        }

        public static Turn Create(LocalDate date, LocalTime[] turn)
        {
            return new Turn(date, turn);
        }

        #endregion
    }
}
