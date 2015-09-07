using MRSES.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace MRSES.Core.Persistence
{
    public interface IEmployeeRepository : IDatabase
    {
        Task<Employee> GetEmployeeAsync(string name_or_id);
        Task<List<string>> GetPositionsAsync();
        Task<List<string>> GetEmployeeNamesByPositionAsync(string position);
        IEmployee Employee { get; set; }
        Task DeleteAsync();
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        #region Properties

        public IEmployee Employee { get; set; }

        #endregion

        #region variables

        #endregion

        #region Constructors

        public EmployeeRepository()
        {
            
        }

        public EmployeeRepository(IEmployee employee) : this()
        {
            Employee = employee;
        }

        #endregion 
        
        #region Methods      

        async public Task SaveAsync()
        {
            ValidateRequiredDataForAction("");

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Save");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Employee.ObjectId);
                    command.Parameters.AddWithValue("location_id", NpgsqlDbType.Text, Employee.Location);
                    command.Parameters.AddWithValue("employee_id", NpgsqlDbType.Text, Employee.Id);
                    command.Parameters.AddWithValue("department", NpgsqlDbType.Text, Employee.Department);
                    command.Parameters.AddWithValue("student", NpgsqlDbType.Boolean, Employee.IsStudent);
                    command.Parameters.AddWithValue("job_type", NpgsqlDbType.Text, Employee.JobType);
                    command.Parameters.AddWithValue("employee_name", NpgsqlDbType.Text, Employee.Name);
                    command.Parameters.AddWithValue("employee_position", NpgsqlDbType.Text, Employee.Position);
                    command.Parameters.AddWithValue("phone_number", NpgsqlDbType.Text, Employee.PhoneNumber);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }             
        }

        async public Task DeleteAsync()
        { 
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Delete");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("table_name", NpgsqlDbType.Text, "employees");
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Employee.ObjectId);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }                      

            Dispose();
        }      

        async public Task<Employee> GetEmployeeAsync(string employee)
        {
            var _employee = new Employee();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployee");
                    command.CommandType = System.Data.CommandType.Text;
                 
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employee);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            _employee.ObjectId = await reader.GetFieldValueAsync<string>(0);
                            _employee.Name = await reader.GetFieldValueAsync<string>(1);
                            _employee.Id = await reader.GetFieldValueAsync<string>(2);
                            _employee.Location = await reader.GetFieldValueAsync<string>(3);
                            _employee.Position = await reader.GetFieldValueAsync<string>(4);
                            _employee.PhoneNumber = await reader.GetFieldValueAsync<string>(5);
                            _employee.JobType = await reader.GetFieldValueAsync<string>(6);
                            _employee.Department = await reader.GetFieldValueAsync<string>(7);
                            _employee.IsStudent = await reader.GetFieldValueAsync<bool>(8);
                        }
                    }
                }
            }

            return _employee;
        }

        async public Task<List<Employee>> SyncEmployeesDataAsync(DateTime lastSyncDate)
        {
            var employees = new List<Employee>();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "SELECT * FROM sync_employees(:date, :access_key)";
                    command.CommandType = System.Data.CommandType.Text;

                    command.Parameters.AddWithValue("date", NpgsqlDbType.Timestamp, lastSyncDate);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employee = new Employee();
                            employee.ObjectId = await reader.GetFieldValueAsync<string>(0);
                            employee.Location = await reader.GetFieldValueAsync<string>(1);
                            employee.Name = await reader.GetFieldValueAsync<string>(2);
                            employee.Id = await reader.GetFieldValueAsync<string>(3);
                            employee.Position = await reader.GetFieldValueAsync<string>(4);
                            employee.PhoneNumber = await reader.GetFieldValueAsync<string>(5);
                            employee.JobType = await reader.GetFieldValueAsync<string>(6);
                            employee.Department = await reader.GetFieldValueAsync<string>(7);
                            employee.IsStudent = await reader.GetFieldValueAsync<bool>(8);

                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;

        }

        async public Task<List<string>> GetPositionsAsync()
        {
            var positions = new List<string>();
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetPositions");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string position = await reader.GetFieldValueAsync<string>(0);
                            positions.Add(position);
                        }
                    }
                }
            }

            return positions;
        }

        async public Task<List<string>> GetEmployeeNamesByPositionAsync(string position)
        {
            var names = new List<string>();
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetNamesByPosition");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("employee_position", NpgsqlDbType.Text, position);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string name = await reader.GetFieldValueAsync<string>(0);
                            names.Add(name);
                        }
                    }
                }
            }

            return names;
        }

        string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "Save":
                    query = @"SELECT add_employee(:object_id, :location_id, :employee_name, :employee_id, :employee_position, :phone_number, :job_type, :department, :student)";
                    break;
                case "Delete":
                    query = @"SELECT delete_record(:table_name, :object_id)";
                    break;
                case "GetEmployee":
                    query = @"SELECT * FROM get_employee_info(:employee)";
                    break;
                case "GetPositions":
                    query = "SELECT get_positions(:access_key)";
                    break;
                case "GetNamesByPosition":
                    query = "SELECT get_names_by_position(:employee_position, :access_key)";
                    break;
                default:
                    break;
            }

            return query;
        }

        static public async Task<string> GetEmployeeObjectIdAsync(string employeeName)
        {
            var objectId = "";

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "SELECT get_employee_object_id(:emp_name, :access_key)";
                    command.CommandType = System.Data.CommandType.Text;

                    command.Parameters.AddWithValue("emp_name", NpgsqlDbType.Text, employeeName);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            objectId = await reader.GetFieldValueAsync<string>(0);
                    }
                }
            }

            if (Shared.StringFunctions.StringIsNullOrEmpty(objectId))
                throw new Exception("El empleado " + employeeName + " no existe.");

            return objectId;
        }

        void ValidateRequiredDataForAction(string dataToValidate)
        {
            if (Employee == null || Employee.Id == "" || Employee.Name == "" || Employee.Position == "" || Employee.Location == "")
                throw new Exception("No se ha completado la información requerida.");

            if (Shared.StringFunctions.StringIsNullOrEmpty(Employee.ObjectId))
                Employee.ObjectId = Shared.StringFunctions.GenerateObjectId(10);
        }

        public void Dispose()
        {
            Employee = null;
        }

        #endregion
    }
}
