using MRSES.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace MRSES.ExternalServices.Postgres
{
    public class EmployeeRepository// : IEmployeeRepository, System.IDisposable
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

        #region Constructors

        public EmployeeRepository()
        {

        }

        public EmployeeRepository(IEmployee employee)
        {
            Employee = employee;
        }

        #endregion 
        
        #region Methods

        async public Task<bool> ExistsAsync()
        {
            ValidateEmployee();
            bool result = false;

            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Exists");
                    command.CommandType = System.Data.CommandType.Text;                    
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Employee.Store);
                    command.Parameters.AddWithValue("employeeId", NpgsqlDbType.Varchar, Employee.ID);
                    await command.Connection.OpenAsync();
                    
                    result = (bool)(await command.ExecuteScalarAsync());
                }
            }

            return result;
        }        

        async public Task SaveAsync()
        {
            bool employeeExists = await ExistsAsync();
            string action = employeeExists == true ? "Update" : "Save";
            await SaveEmployeeInformationAsync(action);                
        }

        async Task SaveEmployeeInformationAsync(string action)
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery(action);
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Employee.Store);
                    command.Parameters.AddWithValue("employeeId", NpgsqlDbType.Varchar, Employee.ID);
                    command.Parameters.AddWithValue("department", NpgsqlDbType.Varchar, Employee.Department);
                    command.Parameters.AddWithValue("isStudent", NpgsqlDbType.Boolean, Employee.IsStudent);
                    command.Parameters.AddWithValue("jobType", NpgsqlDbType.Varchar, Employee.JobType);
                    command.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, Employee.Name);
                    command.Parameters.AddWithValue("employeePosition", NpgsqlDbType.Varchar, Employee.Position);
                    command.Parameters.AddWithValue("phone", NpgsqlDbType.Varchar, Employee.PhoneNumber);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

            Dispose();
        }      
        
        async public Task DeleteAsync()
        {
            bool employeeExists = await ExistsAsync();

            if (!employeeExists)
                RaiseException("Error: El empleado no existe o el ID del empleado no se ha indicado.");
           
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Delete");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Employee.Store);
                    command.Parameters.AddWithValue("employeeId", NpgsqlDbType.Varchar, Employee.ID);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }                      

            Dispose();
        }      

        async public Task<Employee> GetEmployeeAsync(string name)
        {
            var employee = new Employee();

            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployee");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("name", NpgsqlDbType.Varchar, name);                    

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employee.ID = await reader.GetFieldValueAsync<string>(0);
                            employee.Name = await reader.GetFieldValueAsync<string>(1);
                            employee.Position = await reader.GetFieldValueAsync<string>(2);
                            employee.PhoneNumber = await reader.GetFieldValueAsync<string>(3);
                            employee.JobType = await reader.GetFieldValueAsync<string>(4);
                            employee.Department = await reader.GetFieldValueAsync<string>(5);
                            employee.IsStudent = await reader.GetFieldValueAsync<bool>(6);
                            employee.Store = Configuration.StoreLocation;
                        }
                    }
                }
            }

            return employee;
        }

        async public Task<List<string>> GetPositionsAsync()
        {
            var positions = new List<string>();
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetPositions");
                    command.CommandType = System.Data.CommandType.Text;

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

        async public Task<List<Employee>> GetAllEmployeesByPosition(string position)
        {
            var employeeList = new List<Employee>();

            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployeesByPosition");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("employeePosition", NpgsqlDbType.Varchar, position);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employeeList.Add
                            (
                                new Employee 
                                { 
                                    ID = await reader.GetFieldValueAsync<string>(0),
                                    Name = await reader.GetFieldValueAsync<string>(1),
                                    Position = await reader.GetFieldValueAsync<string>(2),
                                    PhoneNumber = await reader.GetFieldValueAsync<string>(3),
                                    JobType = await reader.GetFieldValueAsync<string>(4),
                                    Department = await reader.GetFieldValueAsync<string>(5),
                                    IsStudent = await reader.GetFieldValueAsync<bool>(6),
                                    Store = Configuration.StoreLocation
                                }    
                            );                           
                        }
                    }
                }
            }

            return employeeList;
        }

        async public Task<List<string>> GetEmployeeNamesByPosition(string position)
        {
            var names = new List<string>();
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetNamesByPosition");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("employeePosition", NpgsqlDbType.Varchar, position);

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
                case "Exists":
                    query = "SELECT EXISTS (SELECT TRUE FROM employee WHERE store = :store AND employee_id = :employeeId)";
                    break;
                case "Save":
                    query = @"INSERT INTO employee VALUES (:employeeId, :name, :employeePosition, :phone, :jobType, :department, :isStudent, :store)";
                    break;
                case "Update":
                    query = @"UPDATE employee
                              SET employee_id = :employeeId, name = :name, employee_position = :employeePosition, phone_number = :phone, job_type = :jobType, department = :department, student = :isStudent, store = :store
                              WHERE employee_id = :employeeId";
                    break;
                case "Delete":
                    query = "DELETE FROM employee WHERE store = :store AND employee_id = :employeeId";
                    break;
                case "GetEmployee":
                    query = @"SELECT employee_id, name, employee_position, phone_number, job_type, department, student
                              FROM employee
                              WHERE store = :store AND name = :name";
                    break;
                case "GetPositions":
                    query = "SELECT DISTINCT employee_position FROM employee WHERE store = '" + Configuration.StoreLocation + "'";
                    break;
                case "GetEmployeesByPosition":
                    query = @"SELECT employee_id, name, employee_position, phone_number, job_type, department, student
                              FROM employee
                              WHERE store = :store AND employee_position = :employeePosition";
                    break;
                case "GetNamesByPosition":
                    query = "SELECT name FROM employee WHERE employee_position = :employeePosition AND store = '" + Configuration.StoreLocation + "'";
                    break;
                default:
                    break;
            }

            return query;
        }

        void ValidateEmployee()
        {
            if (Employee == null)
                RaiseException("No se ha especificado empleado");
            else if (Employee.ID == "")
                RaiseException("Valor 0 como ID no es válido");
        }

        void RaiseException(string message)
        {
            throw new System.Exception(message);
        }

        public void Dispose()
        {
            Employee = null;
        }
       
        #endregion    
    }
}
