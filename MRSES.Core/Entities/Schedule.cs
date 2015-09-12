using System.Collections.Generic;
using NodaTime;
using MRSES.Core.Shared;

namespace MRSES.Core.Entities
{
    public interface ISchedule
    {
        string Employee { get; set; }
        LocalDate OfWeek { get; set; }
        Turn[] WeekDays { get; set; }
        double HoursOfWeek { get; }
        byte AmountOfTurns { get; }
    }

    public struct Schedule : ISchedule
    {
        #region variables and properties

        public string Employee { get; set; }
        public LocalDate OfWeek { get; set; }
        public Turn[] WeekDays { get; set; }
        public double HoursOfWeek 
        { 
            get 
            {
                return TotalHoursInWeek();
            } 
        }
        public byte AmountOfTurns { get { return TurnCounter(); } }

        #endregion variables and properties

        #region constructors

        public Schedule(string name) : this(name, DateFunctions.CurrentWeek())
        {
            
        }

        public Schedule(string name, LocalDate ofWeek)
        {
            Employee = name;
            OfWeek = ofWeek;
            WeekDays = new Turn[7];

            int index = 0;

            foreach (var currentDate in DateFunctions.GetWeekDays(OfWeek))
                WeekDays[index++] = new Turn(currentDate);
        }

        #endregion constructors

        #region methods

        double TotalHoursInWeek()
        {
            double totalHours = 0;

            for (int i = 0; i < WeekDays.Length; i++)
            {
                totalHours += WeekDays[i].Hours;
            }
           
            return totalHours;
        }

        byte TurnCounter()
        {
            byte result = 0;

            foreach (var turn in WeekDays)
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
