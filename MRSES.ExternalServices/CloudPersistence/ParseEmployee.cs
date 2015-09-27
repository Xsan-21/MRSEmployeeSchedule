using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;
using MRSES.Core.Entities;
using MRSES.Core.Persistence;
using System.Net.Http;

namespace MRSES.ExternalServices.CloudPersistence
{
    internal class ParseEmployee
    {
        internal async Task SyncAsync()
        {
            await UpdateOrSaveEmployeeAsync();
        }

        async Task UpdateOrSaveEmployeeAsync()
        {
            var employees = await GetEmployeesAsync();

            if (!employees.All(e => string.IsNullOrEmpty(e.ObjectId)))
            {
                foreach (var employee in employees)
                {
                    var update = await UpdateEmployeeAsync(employee);

                    if (!update)
                    {
                        await SaveEmployeeAsync(employee);
                    }
                }
            }
        }

        async Task<List<Employee>> GetEmployeesAsync()
        {
            using (var _employeeRepo = new EmployeeRepository())
            {
                return await _employeeRepo.SyncEmployeesDataAsync(Configuration.LastSyncDate);
            }
        }

        async Task<bool> UpdateEmployeeAsync(IEmployee employee)
        {
            try
            {
                var existingEmployee = await GetEmployeeObject(employee.ObjectId);

                if (existingEmployee == null)
                {
                    return false;
                }

                existingEmployee["employeeId"] = employee.Id;
                existingEmployee["location"] = employee.Location;
                existingEmployee["name"] = employee.Name;
                existingEmployee["jobType"] = employee.JobType;
                existingEmployee["phoneNumber"] = employee.PhoneNumber;
                existingEmployee["position"] = employee.Position;
                existingEmployee["student"] = employee.IsStudent;
                existingEmployee["department"] = employee.Department;

                await existingEmployee.SaveAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("No se pudo establecer conexión a Internet.");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        async Task SaveEmployeeAsync(IEmployee employee)
        {
            try
            {
                var newEmployee = new ParseObject("Employee")
                {
                    { "postgresId", employee.ObjectId },
                    { "employeeId", employee.Id },
                    { "location", employee.Location },
                    { "name", employee.Name },
                    { "jobType", employee.JobType },
                    { "phoneNumber", employee.PhoneNumber },
                    { "position", employee.Position },
                    { "student", employee.IsStudent },
                    { "department", employee.Department }
                };

                await newEmployee.SaveAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("No se pudo establecer conexión a Internet.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static internal async Task<ParseObject> GetEmployeeObject(string postgresId)
        {
            var _object = new ParseObject("Employee");

            try
            {
                _object = await ParseObject.GetQuery("Employee").WhereEqualTo("postgresId", postgresId).FirstOrDefaultAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("No se pudo establecer conexión a Internet.");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _object;
        }
    }
}
