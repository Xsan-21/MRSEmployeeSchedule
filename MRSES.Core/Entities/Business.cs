namespace MRSES.Core.Entities
{
    public interface IBusiness
    {
        string ObjectId { get; set; }
        string Name { get; set; }
        string FirstDayOfWeek { get; set; }
    }

    public struct Business : IBusiness
    {
        public string ObjectId { get; set; }
        public string Name { get; set; }
        public string FirstDayOfWeek { get; set; }

        public Business(string name, string firstDayOfWeek)
        {
            ObjectId = string.Empty;
            Name = name;
            FirstDayOfWeek = firstDayOfWeek;
        }
    }
}
