using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using System.Data;
using System.Linq;
using System;

namespace MRSES.Core.Persistence
{
    public interface IPetitionRepository: IDatabase
    {
        IPetition Petition { get; set; }
        Task DeleteAsync();
        Task<List<Petition>> GetEmployeePetitionsAsync(string employeeName);
        Task<bool> CanDoTheTurnAsync(string employee, Turn turn);
    }

    public class PetitionRepository : IPetitionRepository
    {
        public IPetition Petition { get; set; }

        public PetitionRepository() { }

        public PetitionRepository(IPetition petition)
        {
            Petition = petition;
        }

        public async Task SaveAsync()
        {
            ValidateRequiredDataForAction("Save");

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("SavePetition");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Petition.ObjectId);
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, Petition.Employee);
                    command.Parameters.AddWithValue("pet_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(Petition.Date));
                    command.Parameters.AddWithValue("available_from", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(Petition.AvailableFrom));
                    command.Parameters.AddWithValue("available_to", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(Petition.AvailableTo));

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Petition>> SyncPetitionsDataAsync(DateTime lastSyncDate)
        {
            var petitions = new List<Petition>();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "SELECT * FROM sync_petitions(:sync_date, :access_key)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("sync_date", NpgsqlDbType.Timestamp, lastSyncDate);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var objectId = await reader.GetFieldValueAsync<string>(0);
                            var employee = await reader.GetFieldValueAsync<string>(1);
                            var date = await reader.GetFieldValueAsync<DateTime>(2);
                            var availableFrom = await reader.GetFieldValueAsync<DateTime>(4);
                            var availableTo = await reader.GetFieldValueAsync<DateTime>(5);

                            petitions.Add(
                                new Petition
                                {
                                    ObjectId = objectId,
                                    Employee = employee,
                                    Date = DateFunctions.FromDateTimeToLocalDate(date),
                                    AvailableFrom = DateFunctions.FromDateTimeToLocalTime(availableFrom),
                                    AvailableTo = DateFunctions.FromDateTimeToLocalTime(availableTo)
                                }
                            );
                        }
                    }
                }
            }

            return petitions;
        }

        public async Task DeleteAsync()
        {
            ValidateRequiredDataForAction("Delete");

            var petitionObjectId = await GetPetitionObjectId();
            
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("DeletePetition");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("table_name", NpgsqlDbType.Text, "petitions");
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, petitionObjectId);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // TODO implement a Postgres's Function for this
        async Task<string> GetPetitionObjectId()
        {
            var petitions = await GetEmployeePetitionsAsync(Petition.Employee);
            return petitions.Where(p => p.Date == Petition.Date).Single().ObjectId;
        }

        public async Task<List<Petition>> GetEmployeePetitionsAsync(string employeeName)
        {
            var petitions = new List<Petition>();
            var employeeObjectId = await EmployeeRepository.GetEmployeeObjectIdAsync(employeeName);
         
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployeePetitions");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employeeObjectId);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var objectId = await reader.GetFieldValueAsync<string>(0);
                            var date = await reader.GetFieldValueAsync<DateTime>(2);
                            var availableFrom = await reader.GetFieldValueAsync<DateTime>(4);
                            var availableTo = await reader.GetFieldValueAsync<DateTime>(5);

                            petitions.Add(
                                new Petition
                                {
                                    ObjectId = objectId,
                                    Employee = employeeName,
                                    Date = DateFunctions.FromDateTimeToLocalDate(date),
                                    AvailableFrom = DateFunctions.FromDateTimeToLocalTime(availableFrom),
                                    AvailableTo = DateFunctions.FromDateTimeToLocalTime(availableTo)
                                }
                            );
                        }
                    }
                }
            }

            return petitions;
        }

        public async Task<bool> CanDoTheTurnAsync(string employee, Turn turn)
        {
            bool result = true;

            ValidateRequiredData(employee, turn);

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("VerifyAvailability");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employee);
                    command.Parameters.AddWithValue("turn_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(turn.Date));
                    command.Parameters.AddWithValue("turn", NpgsqlDbType.Array | NpgsqlDbType.Time, turn.GetTurn());
                    
                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                        if (await reader.ReadAsync())
                            result = await reader.GetFieldValueAsync<bool>(0);                       
                }
            }

            return result;
        }

        string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "GetEmployeePetitions":
                    query = "SELECT * FROM get_employee_petitions(:employee)";
                    break;
                case "SavePetition":
                    query = "SELECT add_petition(:object_id, :employee, :pet_date, :available_from, :available_to)";
                    break;
                case "DeletePetition":
                    query = "SELECT delete_record(:table_name, :object_id)";
                    break;
                case "VerifyAvailability":
                    query = "SELECT petition_employee_can_do_the_turn(:employee, :turn_date, :turn)";
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

        void ValidateRequiredDataForAction(string actionToValidate)
        {
            if(actionToValidate == "Save")
            {
                if (Petition == null)
                    throw new NullReferenceException("No se ha indicado petiticion.");

                if (StringFunctions.StringIsNullOrEmpty(Petition.ObjectId))
                    Petition.ObjectId = StringFunctions.GenerateObjectId(10);

                if (StringFunctions.StringIsNullOrEmpty(Petition.Employee))
                    throw new Exception("No se ha indicado empleado");

                if (Petition.Date == new NodaTime.LocalDate())
                    throw new Exception("No se ha indicado fecha en la petition");
            } 
        }

        void ValidateRequiredData(string employee, ITurn turn)
        {
            if (StringFunctions.StringIsNullOrEmpty(employee))
                throw new System.ArgumentException("No se ha indicado empleado");

            if(turn == null)
                throw new System.NullReferenceException ("No ha indicado turno para verificar disponibilidad.");

            if (turn.Date == new NodaTime.LocalDate())
                throw new System.ArgumentException("No ha indicado fecha para verificar disponibilidad.");
        }
    }
}
