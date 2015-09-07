using NodaTime;

namespace MRSES.Core.Entities
{
    public interface IPetition
    {
        string ObjectId { get; set; }
        string Employee { get; set; }
        LocalDate Date { get; set; }
        bool FreeDay { get; }
        LocalTime AvailableFrom { get; set; }
        LocalTime AvailableTo { get; set; }
    }

    public struct Petition : IPetition
    {
        public string ObjectId { get; set; }
        public string Employee { get; set; }
        public LocalDate Date { get; set; }
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
