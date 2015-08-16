using NodaTime;

namespace MRSES.Core.Entities
{
    public interface IPetition
    {
        string ObjectID { get; set; }
        LocalDate Date { get; set; }
        bool FreeDay { get; }
        LocalTime AvailableFrom { get; set; }
        LocalTime AvailableTo { get; set; }
    }

    public struct Petition : IPetition
    {
        public string ObjectID { get; set; }
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
