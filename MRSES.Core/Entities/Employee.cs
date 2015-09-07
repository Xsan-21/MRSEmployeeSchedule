namespace MRSES.Core.Entities
{
    public interface IEmployee
    {
        string ObjectId { get; set; }
        string Location { get; set; }
        string Name { get; set; }
        string Id { get; set; }
        string PhoneNumber { get; set; }
        string Position { get; set; }
        string JobType { get; set; }
        bool IsStudent { get; set; }
        string Department { get; set; }
    }

    public class Employee : IEmployee
    {
        #region PROPERTIES

        public string ObjectId { get; set; }
        public string Location { get; set; } 
        public string Name { get; set; }
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public string JobType { get; set; }
        public bool IsStudent { get; set; }
        public string Department { get; set; }

        #endregion

        #region CONSTRUCTORS

        public Employee() { }
        public Employee(string objectId, string location, string name, string id, string phone, string position, string jobType, string department, bool isStudent)
        {
            ObjectId = objectId;
            Location = location;
            Name = name;
            Id = id;
            PhoneNumber = phone;
            Position = position;
            JobType = jobType;
            Department = department;
            IsStudent = isStudent;
        }

        #endregion       
    }
}
