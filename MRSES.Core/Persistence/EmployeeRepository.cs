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
        Task<string> GetEmployeeObjectIdAsync(string employeeName);
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
            ValidateRequiredData("");

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Save");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Employee.ObjectID);
                    command.Parameters.AddWithValue("location_id", NpgsqlDbType.Text, Employee.Location);
                    command.Parameters.AddWithValue("employee_id", NpgsqlDbType.Text, Employee.ID);
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
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Employee.ObjectID);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }                      

            Dispose();
        }      

        async public Task<Employee> GetEmployeeAsync(string name_or_id)
        {
            var employee = new Employee();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployee");
                    command.CommandType = System.Data.CommandType.Text;
                 
                    command.Parameters.AddWithValue("employee_name_or_id", NpgsqlDbType.Text, name_or_id);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employee.ObjectID = await reader.GetFieldValueAsync<string>(0);
                            employee.Name = await reader.GetFieldValueAsync<string>(1);
                            employee.ID = await reader.GetFieldValueAsync<string>(2);
                            employee.Position = await reader.GetFieldValueAsync<string>(3);
                            employee.PhoneNumber = await reader.GetFieldValueAsync<string>(4);
                            employee.JobType = await reader.GetFieldValueAsync<string>(5);
                            employee.Department = await reader.GetFieldValueAsync<string>(6);
                            employee.IsStudent = await reader.GetFieldValueAsync<bool>(7);
                        }
                    }
                }
            }

            return employee;
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

        public string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "Save":
                    query = @"SELECT add_employee(:object_id, :location_id, :employee_name, :employee_id, :employee_position, :phone_number, :job_type, :department, :student)";
                    break;
                case "Delete":
                    query = @"SELECT delete_employee(:object_id)";
                    break;
                case "GetEmployee":
                    query = @"SELECT * FROM get_employee_info(:employee_name_or_id, :access_key)";
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

        async public Task<string> GetEmployeeObjectIdAsync(string employeeName)
        {
            var employeeId = "";

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
                            employeeId = await reader.GetFieldValueAsync<string>(0);
                    }
                }
            }

            return employeeId;
        }

        public void ValidateRequiredData(string dataToValidate)
        {
            if (Employee == null || Employee.ID == "" || Employee.Name == "" || Employee.Position == "")
                throw new Exception("No se ha completado la información requerida.");

            if (Shared.StringFunctions.StringIsNullOrEmpty(Employee.ObjectID))
                Employee.ObjectID = Shared.StringFunctions.GenerateObjectId(10);
        }

        public void Dispose()
        {
            Employee = null;
        }

        #endregion
    }
}
