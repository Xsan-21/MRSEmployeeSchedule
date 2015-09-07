using MRSES.Core.Entities;
using MRSES.Core.Shared;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Threading.Tasks;

namespace MRSES.Core.Persistence
{
    public interface IAvailabilityRepository : IDatabase
    {
        IAvailability Availability { get; set; }
        Task<Availability> GetAvailabilityAsync(string employeeName);
        Task<string> GetAvailabilityAsync(string employeeName, NodaTime.IsoDayOfWeek dayOfWeek);
        Task<bool> CanDoTheTurnAsync(string employeeName, Turn turn);
    }

    public class AvailabilityRepository : IAvailabilityRepository
    {
        // TODO the method ValidateRequiredData(employeeName, Iturn turn) should be added, as is in PetitionRepository.
        #region Properties

        public IAvailability Availability { get; set; }

        #endregion

        #region Constructors

        public AvailabilityRepository() { }

        public AvailabilityRepository(IAvailability availability)
        {
            Availability = availability;
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
                    command.CommandText = GetQuery("SaveAvailability");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Availability.ObjectId);
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, Availability.Employee);
                    command.Parameters.AddWithValue("monday", NpgsqlDbType.Text, Availability.Monday);
                    command.Parameters.AddWithValue("tuesday", NpgsqlDbType.Text, Availability.Tuesday);
                    command.Parameters.AddWithValue("wednesday", NpgsqlDbType.Text, Availability.Wednesday);
                    command.Parameters.AddWithValue("thursday", NpgsqlDbType.Text, Availability.Thursday);
                    command.Parameters.AddWithValue("friday", NpgsqlDbType.Text, Availability.Friday);
                    command.Parameters.AddWithValue("saturday", NpgsqlDbType.Text, Availability.Saturday);
                    command.Parameters.AddWithValue("sunday", NpgsqlDbType.Text, Availability.Sunday);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

            Dispose();
        }

        async public Task<Availability> GetAvailabilityAsync(string employee)
        {
            var employeeAvailability = new Availability();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetAvailability");
                    command.CommandType = System.Data.CommandType.Text;
                 
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employee);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employeeAvailability.ObjectId = await reader.GetFieldValueAsync<string>(0);
                            employeeAvailability.Employee = await reader.GetFieldValueAsync<string>(1);
                            employeeAvailability.Monday = await reader.GetFieldValueAsync<string>(2);
                            employeeAvailability.Tuesday = await reader.GetFieldValueAsync<string>(3);
                            employeeAvailability.Wednesday = await reader.GetFieldValueAsync<string>(4);
                            employeeAvailability.Thursday = await reader.GetFieldValueAsync<string>(5);
                            employeeAvailability.Friday = await reader.GetFieldValueAsync<string>(6);
                            employeeAvailability.Saturday = await reader.GetFieldValueAsync<string>(7);
                            employeeAvailability.Sunday = await reader.GetFieldValueAsync<string>(8);
                        }
                    }
                }
            }

            return employeeAvailability;
        }

        async public Task<System.Collections.Generic.List<Availability>> SyncAvailabilitiesDataAsync(DateTime sync_date)
        {
            var availabilities = new System.Collections.Generic.List<Availability>();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "SELECT * FROM sync_availabilities(:sync_date, :access_key)";
                    command.CommandType = System.Data.CommandType.Text;

                    command.Parameters.AddWithValue("sync_date", NpgsqlDbType.Timestamp, sync_date);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var employeeAvailability = new Availability()
                            {
                                ObjectId = await reader.GetFieldValueAsync<string>(0),
                                Employee = await reader.GetFieldValueAsync<string>(1),
                                Monday = await reader.GetFieldValueAsync<string>(2),
                                Tuesday = await reader.GetFieldValueAsync<string>(3),
                                Wednesday = await reader.GetFieldValueAsync<string>(4),
                                Thursday = await reader.GetFieldValueAsync<string>(5),
                                Friday = await reader.GetFieldValueAsync<string>(6),
                                Saturday = await reader.GetFieldValueAsync<string>(7),
                                Sunday = await reader.GetFieldValueAsync<string>(8)
                            };

                            availabilities.Add(employeeAvailability);
                        }
                    }
                }
            }

            return availabilities;
        }

        async public Task<string> GetAvailabilityAsync(string employeeName, NodaTime.IsoDayOfWeek dayOfWeek)
        {
            var availability = "";

            var employeeAvailability = await GetAvailabilityAsync(employeeName);

            switch (dayOfWeek)
            {
                case NodaTime.IsoDayOfWeek.Friday:
                    availability = employeeAvailability.Friday;
                    break;
                case NodaTime.IsoDayOfWeek.Monday:
                    availability = employeeAvailability.Monday;
                    break;
                case NodaTime.IsoDayOfWeek.Saturday:
                    availability = employeeAvailability.Saturday;
                    break;
                case NodaTime.IsoDayOfWeek.Sunday:
                    availability = employeeAvailability.Sunday;
                    break;
                case NodaTime.IsoDayOfWeek.Thursday:
                    availability = employeeAvailability.Thursday;
                    break;
                case NodaTime.IsoDayOfWeek.Tuesday:
                    availability = employeeAvailability.Tuesday;
                    break;
                case NodaTime.IsoDayOfWeek.Wednesday:
                    availability = employeeAvailability.Wednesday;
                    break;
                default:
                    availability = "available";
                    break;
            }

            return availability;
        }

        public async Task<bool> CanDoTheTurnAsync(string employeeName, Turn turn)
        {
            bool availability = true;
            var employeeObjectId = await EmployeeRepository.GetEmployeeObjectIdAsync(employeeName);

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("VerifyAvailability");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employeeObjectId);
                    command.Parameters.AddWithValue("turn_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(turn.Date));
                    command.Parameters.AddWithValue("turn", NpgsqlDbType.Array | NpgsqlDbType.Time, turn.GetTurn());

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            availability = await reader.GetFieldValueAsync<bool>(0);
                        }
                    }
                }
            }

            return availability;
        }

        string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "GetAvailability":
                    query = "SELECT * FROM get_availability(:employee)";
                    break;
                case "SaveAvailability":
                    query = "SELECT add_availability(:object_id, :employee, :monday, :tuesday, :wednesday, :thursday, :friday, :saturday, :sunday)";
                    break;
                case "VerifyAvailability":
                    query = "SELECT availability_employee_can_do_the_turn(:employee, :turn_date, :turn)";
                    break;
                default:
                    break;
            }

            return query;
        }
      
        public void Dispose()
        {
            Availability = null;
        }

        public void ValidateRequiredDataForAction(string dataToValidate)
        {
            if (Availability == null)
                throw new Exception("No se ha indicado la disponibilidad del empleado.");

            if (StringFunctions.StringIsNullOrEmpty(Availability.Employee))
                throw new Exception("No se ha especificado el object id del empleado");
        }

        #endregion
    }
}
