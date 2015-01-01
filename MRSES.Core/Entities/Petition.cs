using NodaTime;

namespace MRSES.Core.Entities
{
    public interface IPetition
    {
        LocalDate Date { get; set; }
        string EmployeeName { get; set; }
        bool FreeDay { get; }
        LocalTime AvailableFrom { get; set; }
        LocalTime AvailableTo { get; set; }
    }

    public struct Petition : IPetition
    {
        public LocalDate Date { get; set; }
        public string EmployeeName { get; set; }
        public bool FreeDay
        {
            get
            {
                return Period.Between(AvailableFrom, AvailableTo).Hours > 0 ? false : true;
            }
        }
        public LocalTime AvailableFrom { get; set; }
        public LocalTime AvailableTo { get; set; }
    }
}
