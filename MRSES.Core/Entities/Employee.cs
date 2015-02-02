namespace MRSES.Core.Entities
{
    public interface IEmployee
    {
        string Name { get; set; }
        string ID { get; set; }
        string PhoneNumber { get; set; }
        string Position { get; set; }
        string JobType { get; set; }
        bool IsStudent { get; set; }
        string Department { get; set; }
        string Store { get; set; }
        string OldID { get; set; }
    }

    public class Employee : IEmployee
    {
        #region PROPERTIES

        public string Name { get; set; }
        public string ID { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public string JobType { get; set; }
        public bool IsStudent { get; set; }
        public string Department { get; set; }
        public string Store { get; set; }
        public string OldID { get; set; }

        #endregion

        #region CONSTRUCTORS

        public Employee() { }
        public Employee(string name, string id, string phone, string position, string jobType, string department, string store, bool isStudent, string oldNameOrID)
        {
            Name = name;
            ID = id;
            PhoneNumber = phone;
            Position = position;
            JobType = jobType;
            Department = department;
            Store = store;
            IsStudent = isStudent;
            OldID = oldNameOrID;
        }

        #endregion       
    }
}
