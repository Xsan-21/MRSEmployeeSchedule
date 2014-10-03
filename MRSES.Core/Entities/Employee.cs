namespace MRSES.Core.Entities
{
    public interface IEmployee
    {
        string Name { get; set; }
        int ID { get; set; }
        string PhoneNumber { get; set; }
        string Position { get; set; }
        string JobType { get; set; }
        bool IsStudent { get; set; }
        string Department { get; set; }
        string Store { get; set; } 
    }

    public class Employee : IEmployee
    {
        #region VARIABLES AND PROPERTIES

        string _name, _phone, _position, _jobType, _department, _store;
        int _id;
        bool _isStudent;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public string PhoneNumber
        {
            get { return _phone; }
            set { _phone = value; }
        }
        public string Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public string JobType
        {
            get { return _jobType; }
            set { _jobType = value; }
        }
        public string Department
        {
            get { return _department; }
            set { _department = value; }
        }
        public string Store
        {
            get { return _store; }
            set { _store = value; }
        }
        public bool IsStudent
        {
            get { return _isStudent; }
            set { _isStudent = value; }
        }

        #endregion

        #region CONSTRUCTORS

        public Employee() { }
        public Employee(string name, int id, string phone, string position, string jobType, string department, bool isStudent, string store)
        {
            Name = name;
            ID = id;
            PhoneNumber = phone;
            Position = position;
            JobType = jobType;
            Department = department;
            IsStudent = isStudent;
            Store = store;
        }

        #endregion       
    }
}
