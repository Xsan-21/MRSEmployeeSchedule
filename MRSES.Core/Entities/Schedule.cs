using System.Collections.Generic;
using NodaTime;
using MRSES.Core.Shared;

namespace MRSES.Core.Entities
{
    public class Schedule
    {
        #region variables and properties

        List<Turn> _turns;

        public string Name { get; set; }
        public LocalDate OfWeek { get; set; }
        public List<Turn> Turns
        {
            get { return _turns; }
            set
            {
                if (value == null) return;
                _turns = value;
            }
        }

        public byte AmountOfTurns { get { return TurnCounter(); } }

        #endregion variables and properties

        #region constructors

        public Schedule() : this(DateFunctions.CurrentWeek(), string.Empty)
        {
            
        }

        public Schedule(LocalDate ofWeek, string name)
        {
            Name = name;
            OfWeek = ofWeek;

            Turns = new List<Turn>();

            foreach (var currentDate in DateFunctions.GetWeekDays(OfWeek))
                Turns.Add(new Turn(currentDate));
        }

        #endregion constructors

        #region methods

        byte TurnCounter()
        {
            byte result = 0;

            foreach (var turn in Turns)
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
