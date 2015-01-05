﻿using MRSES.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace MRSES.Core.Persistence
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

        async public Task SaveAsync()
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Save");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("employee_id", NpgsqlDbType.Varchar, Employee.ID);
                    command.Parameters.AddWithValue("department", NpgsqlDbType.Varchar, Employee.Department);
                    command.Parameters.AddWithValue("student", NpgsqlDbType.Boolean, Employee.IsStudent);
                    command.Parameters.AddWithValue("job_type", NpgsqlDbType.Varchar, Employee.JobType);
                    command.Parameters.AddWithValue("employee_name", NpgsqlDbType.Varchar, Employee.Name);
                    command.Parameters.AddWithValue("employee_position", NpgsqlDbType.Varchar, Employee.Position);
                    command.Parameters.AddWithValue("phone_number", NpgsqlDbType.Varchar, Employee.PhoneNumber);
                    command.Parameters.AddWithValue("old_id", NpgsqlDbType.Varchar, Employee.OldID);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

            Dispose();               
        }

        async public Task DeleteAsync()
        { 
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Delete");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("employee_id", NpgsqlDbType.Varchar, Employee.ID);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }                      

            Dispose();
        }      

        async public Task<Employee> GetEmployeeAsync(string name_or_id)
        {
            var employee = new Employee();

            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployee");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("employee_name_or_id", NpgsqlDbType.Varchar, name_or_id);                    

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employee.Name = await reader.GetFieldValueAsync<string>(0);
                            employee.ID = await reader.GetFieldValueAsync<string>(1);
                            employee.Position = await reader.GetFieldValueAsync<string>(2);
                            employee.PhoneNumber = await reader.GetFieldValueAsync<string>(3);
                            employee.JobType = await reader.GetFieldValueAsync<string>(4);
                            employee.Department = await reader.GetFieldValueAsync<string>(5);
                            employee.IsStudent = await reader.GetFieldValueAsync<bool>(6);
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
                    command.Parameters.AddWithValue(":store", NpgsqlDbType.Varchar, Configuration.StoreLocation);

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

            return positions.OrderBy(name => name).ToList();
        }

        async public Task<List<string>> GetEmployeeNamesByPositionAsync(string position)
        {
            var names = new List<string>();
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetNamesByPosition");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("employeePosition", NpgsqlDbType.Varchar, position);
                    command.Parameters.AddWithValue("store", NpgsqlDbType.Varchar, Configuration.StoreLocation);

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
                    query = @"SELECT add_employee(:employee_name, :employee_id, :employee_position, :phone_number, :job_type, :department, :student, :store, :old_id)";
                    break;
                case "Delete":
                    query = @"SELECT delete_employee(:employee_id, :store)";
                    break;
                case "GetEmployee":
                    query = @"SELECT * FROM get_employee_info(:employee_name_or_id, :store)";
                    break;
                case "GetPositions":
                    query = "SELECT get_positions(:store)";
                    break;
                case "GetNamesByPosition":
                    query = "SELECT get_names_by_position(:employeePosition, :store)";
                    break;
                default:
                    break;
            }

            return query;
        }

        void ValidateEmployee()
        {
            if (Employee == null || (Employee.ID == "" && Employee.Name == ""))
                RaiseException("No se ha especificado empleado");
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
