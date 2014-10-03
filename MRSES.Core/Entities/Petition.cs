using NodaTime;

namespace MRSES.Core.Entities
{
    public interface IPetition
    {
        LocalDate Date { get; set; }
        IEmployee Employee { get; set; }
        bool FreeDay { get; }
        LocalDateTime AvailableFrom { get; set; }
        LocalDateTime AvailableTo { get; set; }
    }

    public struct Petition : IPetition
    {
        public LocalDate Date { get; set; }
        public IEmployee Employee { get; set; }
        public bool FreeDay
        {
            get
            {
                return Period.Between(AvailableFrom, AvailableTo).Hours > 0 ? false : true;
            }
        }
        public LocalDateTime AvailableFrom { get; set; }
        public LocalDateTime AvailableTo { get; set; }
    }
}
