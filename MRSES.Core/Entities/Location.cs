namespace MRSES.Core.Entities
{
    public interface ILocation
    {
        string ObjectId { get; set; }
        string Business { get; set; }
        string AccessKey { get; set; }
        string City { get; set; }
        string PhoneNumber { get; set; }
    }

    public struct Location : ILocation
    {
        public string ObjectId { get; set; }
        public string Business {get;set;}
        public string AccessKey { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }

        public Location(string accessKey, string city, string phoneNumber)
        {
            ObjectId = string.Empty;
            Business = string.Empty;
            AccessKey = accessKey;
            City = city;
            PhoneNumber = phoneNumber;
        }
    }
}
