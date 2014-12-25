using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MRSES.Core;
using MRSES.Core.Entities;
using Parse;

namespace MRSES.ExternalServices.Parse
{
    public class EmployeeRepository : IEmployeeRepository, System.IDisposable
    {
        #region Fields

        private IEmployee _employee;
        #endregion

        #region Properties

        public IEmployee Employee
        {
            private get { return _employee; }
            set 
            { 
                _employee = value;
            }
        } 

        #endregion

        #region CONSTRUCTORS

        public EmployeeRepository()
        {
            
        }

        public EmployeeRepository(IEmployee employee)
        {
            Employee = employee;            
        }

        #endregion          

        #region Methods
        async public Task SaveAsync()
        {
            if (await ExistsAsync())
                await UpdateAsync();
            else
                await SaveNewEmployeeAsync();           
        }

        async Task SaveNewEmployeeAsync()
        {
            var newEmployee = new ParseObject("Employees")
	        {
		        { "Id", Employee.ID },
		        { "name", Employee.Name },
		        { "position", Employee.Position },
		        { "phoneNumber", Employee.PhoneNumber },
		        { "jobType", Employee.JobType },
                { "student", Employee.IsStudent},
		        { "department", Employee.Department}, 
                { "store", Configuration.StoreLocation}
	        };

            await newEmployee.SaveAsync(); 
            Dispose();
        }

        async Task UpdateAsync()
        {
            ValidateEmployee();
            var existingEmployee = await GetEmployeeObjectWithoutData(Employee);
            existingEmployee["name"] = Employee.Name;
            existingEmployee["position"] = Employee.Position;
            existingEmployee["phoneNumber"] = Employee.PhoneNumber;
            existingEmployee["jobType"] = Employee.JobType;
            existingEmployee["student"] = Employee.IsStudent;
            existingEmployee["department"] = Employee.Department;
            existingEmployee["store"] = Configuration.StoreLocation;

            await existingEmployee.SaveAsync();
            Dispose();
        }
        
        async public Task DeleteAsync()
        {
            var existingEmployee = await GetEmployeeObjectWithoutData(Employee);

            if (existingEmployee == null)
                RaiseException("No existe el empleado que se intento eliminar");

            await existingEmployee.DeleteAsync();

            Dispose();
        }        

        async public Task<bool> ExistsAsync()
        {
            ValidateEmployee();
            var query = from n in ParseObject.GetQuery("Employees")
                        where n.Get<string>("store") == Configuration.StoreLocation
                        where n.Get<string>("Id") == Employee.ID
                        select n;

            return await query.FirstOrDefaultAsync() == null ? false : true;
        }

        async public Task<Employee> GetEmployeeAsync(string name_or_id)
        {
            var query = from employee in ParseObject.GetQuery("Employees")
                        where employee.Get<string>("store") == Configuration.StoreLocation
                        where employee.Get<string>("name") == name_or_id || employee.Get<string>("Id") == name_or_id                         
                        select employee;

            var employeeInfo = await query.FirstOrDefaultAsync();

            if (employeeInfo == null)
                return null;

            return await FillEmployeeInformation(employeeInfo);
        }

        async static internal Task<Employee> CreateEmployee(ParseObject employee)
        {
            if (employee.ClassName != "Employees" || employee == null)
                RaiseStaticException("The given employee parameter is not from Employee class or is null");

            return await FillEmployeeInformation(employee);
        }

        async static internal Task<ParseObject> GetEmployeeObjectWithoutData(IEmployee employee)
        {
            if(employee == null)
                RaiseStaticException("The given employee parameter object is null or employee does not exists");

            string employeeObjectId = await GetEmployeeObjectId(employee);

            if (employeeObjectId == null)
                RaiseStaticException("The requested employee does not exists");

            return ParseObject.CreateWithoutData("Employees", employeeObjectId);
        }

        async static Task<string> GetEmployeeObjectId(IEmployee employee)
        {
            var query = from employees in ParseObject.GetQuery("Employees")
                        where employees.Get<string>("store") == Configuration.StoreLocation
                        where employees.Get<string>("Id") == employee.ID && 
                        employees.Get<string>("name") == employee.Name
                        select employees;

            var result = await query.FirstOrDefaultAsync();

            return result == null ? null : result.ObjectId;
        }

        async public Task<List<string>> GetPositionsAsync()
        {
            return await Task.Run(async () => 
            {
                var query = from employee in ParseObject.GetQuery("Employees")
                            where employee.Get<string>("store") == Configuration.StoreLocation
                            orderby employee.Get<string>("position")
                            select employee;

                var employees = await query.FindAsync();
                var result = employees.Select(x => x.Get<string>("position")).Distinct();

                return result.ToList();
            });           
        }

        async public Task<List<string>> GetAllEmployeeNamesWithId()
        {
            return await Task.Run(async () => { 
                var query = from employee in ParseObject.GetQuery("Employees")
                            where employee.Get<string>("store") == Configuration.StoreLocation
                            orderby employee.Get<string>("name")
                            select employee;

                var employees = await query.FindAsync();

                var employeeNameWithId = new List<string>();

                foreach (var employee in employees)
                {
                    string result = string.Format("{0}, {1}", employee.Get<string>("name"), employee.Get<int>("Id"));
                    employeeNameWithId.Add(result);
                }

                return employeeNameWithId;
            });
        }

        async public Task<List<string>> GetEmployeeNamesByPositionAsync(string position)
        {
            return await Task.Run(async () =>
            {
                var employeeList = new List<string>();

                var employees = from employee in ParseObject.GetQuery("Employees")
                                where employee.Get<string>("store") == Configuration.StoreLocation
                                where employee.Get<string>("position") == position
                                orderby employee.Get<string>("name")
                                select employee;

                foreach (var employee in await employees.FindAsync())
                    employeeList.Add(employee.Get<string>("name"));

                return employeeList;
            });
        }

        static Task<Employee> FillEmployeeInformation(ParseObject employee)
        {
            return Task.Run(() => 
            {
                return new Employee()
                {
                    Department = employee.Get<string>("department"),
                    ID = employee.Get<string>("Id"),
                    Name = employee.Get<string>("name"),
                    IsStudent = employee.Get<bool>("student"),
                    JobType = employee.Get<string>("jobType"),
                    PhoneNumber = employee.Get<string>("phoneNumber"),
                    Position = employee.Get<string>("position")
                };
            });            
        }

        void RaiseException(string message)
        {
            throw new System.Exception(message);
        }

        static void RaiseStaticException(string message)
        {
            throw new System.Exception(message);
        }

        void ValidateEmployee()
        {
            if (Employee == null)
                RaiseException("No se ha especificado empleado");
            else if (Employee.ID == "")
                RaiseException("Valor 0 como ID no es válido");
        }

        static void ValidateEmployee(IEmployee employee)
        {
            if (employee == null)
                RaiseStaticException("No se ha especificado empleado");
            else if (employee.ID == "")
                RaiseStaticException("Valor en blanco como ID no es válido");
        }

        public void Dispose()
        {
            Employee = null;
        }

        #endregion    
    }
}
