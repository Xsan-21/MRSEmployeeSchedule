using System.Collections.Generic;
using NodaTime;

namespace MRSES.Core.Entities
{
    public class Worker
    {
        #region variables and properties

        List<Turn> _schedule;

        public string Name { get; set; }
        private LocalDate _ofWeek;
        public LocalDate OfWeek { get { return _ofWeek; } set { _ofWeek = value; } }
        public List<Turn> Schedule
        {
            get { return _schedule; }
            set
            {
                if (value == null) return;
                _schedule = value;
            }
        }
        public byte AmountOfTurns { get { return TurnCounter(); } }

        #endregion variables and properties

        #region constructors

        public Worker(LocalDate ofWeek, string name = "")
        {
            Name = name;
            OfWeek = ofWeek;
            Schedule = new List<Turn>();

            foreach (var currentDate in WorkWeek.GetWeekDays(OfWeek))
                Schedule.Add(new Turn(OfWeek, currentDate));
        }

        #endregion constructors

        #region methods

        byte TurnCounter()
        {
            byte result = 0;

            foreach (var turn in Schedule)
            {
                if (turn.Hours != 0)
                {
                    if (turn.Hours <= 5)
                        result += 1;
                    else
                        result += 2;
                }
            }

            return result;
        }

        #endregion methods
    }
}
