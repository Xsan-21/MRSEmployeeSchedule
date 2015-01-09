using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using System.Data;

namespace MRSES.Core.Persistence
{
    public class PetitionRepository : IAvailable, System.IDisposable
    {
        public IPetition Petition { get; set; }

        public PetitionRepository() { }

        public PetitionRepository(IPetition petition)
        {
            Petition = petition;
        }

        public async Task SaveAsync()
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("SavePetition");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("emp_name", NpgsqlDbType.Varchar, Petition.EmployeeName);
                    command.Parameters.AddWithValue("emp_store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("pet_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(Petition.Date));
                    command.Parameters.AddWithValue("available_from", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(Petition.AvailableFrom));
                    command.Parameters.AddWithValue("available_to", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(Petition.AvailableTo));

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

            Dispose();
        }

        public async Task DeleteAsync()
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("DeletePetition");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("emp_name", NpgsqlDbType.Varchar, Petition.EmployeeName);
                    command.Parameters.AddWithValue("emp_store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("pet_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(Petition.Date));

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

            Dispose();
        }

        public async Task<List<Petition>> GetEmployeePetitionsAsync(string employeeName)
        {
            var petitions = new List<Petition>();
         
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployeePetitions");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("emp_store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("emp_name", NpgsqlDbType.Varchar, employeeName);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var date = await reader.GetFieldValueAsync<DateTime>(0);
                            var availableFrom = await reader.GetFieldValueAsync<DateTime>(2);
                            var availableTo = await reader.GetFieldValueAsync<DateTime>(3);

                            petitions.Add(
                                new Petition
                                {
                                    EmployeeName = employeeName,
                                    Date = new NodaTime.LocalDate(date.Year, date.Month, date.Day),
                                    AvailableFrom = new NodaTime.LocalTime(availableFrom.Hour, availableFrom.Minute, availableFrom.Second),
                                    AvailableTo = new NodaTime.LocalTime(availableTo.Hour, availableTo.Minute, availableTo.Second)
                                }
                            );
                        }
                    }
                }
            }

            return petitions;
        }

        public async Task<bool> CanDoTheTurnAsync(string employeeName, ITurn turn)
        {
            bool result = true;
            var employeeId = await EmployeeRepository.GetEmployeeIdAsync(employeeName);

            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("VerifyAvailability");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("emp_id", NpgsqlDbType.Varchar, employeeId);
                    command.Parameters.AddWithValue("turn_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(turn.Date));
                    command.Parameters.AddWithValue("turn_in1", NpgsqlDbType.Time, FromLocalTimeToDateTime(turn.TurnIn1));
                    command.Parameters.AddWithValue("turn_out1", NpgsqlDbType.Time, FromLocalTimeToDateTime(turn.TurnOut1));
                    command.Parameters.AddWithValue("turn_in2", NpgsqlDbType.Time, FromLocalTimeToDateTime(turn.TurnIn2));
                    command.Parameters.AddWithValue("turn_out2", NpgsqlDbType.Time, FromLocalTimeToDateTime(turn.TurnOut2));

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result = await reader.GetFieldValueAsync<bool>(0);
                        }
                    }
                }
            }

            return result;
        }

        DateTime FromLocalTimeToDateTime(NodaTime.LocalTime time)
        {
            return DateFunctions.FromLocalTimeToDateTime(time);
        }

        static string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "GetEmployeePetitions":
                    query = "SELECT * FROM get_employee_petitions(:emp_name, :emp_store)";
                    break;
                case "SavePetition":
                    query = "SELECT add_petition(:emp_name, :emp_store, :pet_date, :available_from, :available_to)";
                    break;
                case "DeletePetition":
                    query = "SELECT delete_petition(:emp_name, :emp_store, :pet_date)";
                    break;
                case "VerifyAvailability":
                    query = "SELECT petition_employee_can_do_the_turn(:emp_id, :turn_date, :turn_in1, :turn_out1, :turn_in2, :turn_out2)";
                    break;
                default:
                    break;
            }

            return query;
        }

        public void Dispose()
        {
            Petition = null;
        }
    }
}
