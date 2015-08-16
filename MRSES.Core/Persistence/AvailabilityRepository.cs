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
            ValidateRequiredData("");

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("SaveAvailability");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Availability.ObjectID);
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, Availability.EmployeeObjectID);
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

        async public Task<Availability> GetAvailabilityAsync(string employeeName)
        {
            
            var employeeAvailability = new Availability();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetAvailability");
                    command.CommandType = System.Data.CommandType.Text;
                 
                    command.Parameters.AddWithValue("employee_name", NpgsqlDbType.Text, employeeName);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employeeAvailability.ObjectID = await reader.GetFieldValueAsync<string>(0);
                            employeeAvailability.EmployeeObjectID = await reader.GetFieldValueAsync<string>(1);
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
            var employeeObjectId = await new EmployeeRepository().GetEmployeeObjectIdAsync(employeeName);

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

        public string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "GetAvailability":
                    query = "SELECT * FROM get_availability(:employee_name, :access_key)";
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

        public void ValidateRequiredData(string dataToValidate)
        {
            if (Availability == null)
                throw new Exception("No se ha indicado la disponibilidad del empleado.");

            if (StringFunctions.StringIsNullOrEmpty(Availability.EmployeeObjectID))
                throw new Exception("No se ha especificado el object id del empleado");
        }

        #endregion
    }
}
