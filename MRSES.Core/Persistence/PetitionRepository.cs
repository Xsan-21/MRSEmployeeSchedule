using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using System.Data;
using System.Linq;

namespace MRSES.Core.Persistence
{
    public interface IPetitionRepository: IDatabase
    {
        string EmployeeName { get; set; }
        IPetition Petition { get; set; }
        Task DeleteAsync();
        Task<List<Petition>> GetEmployeePetitionsAsync(string employeeName);
        Task<bool> CanDoTheTurnAsync(string employeeName, Turn turn);
    }

    public class PetitionRepository : IPetitionRepository
    {
        public string EmployeeName { get; set; }
        public IPetition Petition { get; set; }

        public PetitionRepository() { }

        public PetitionRepository(string employeeName, IPetition petition)
        {
            EmployeeName = employeeName;
            Petition = petition;
        }

        async Task<string> GetEmployeeObjectIdAsync(string employee)
        {
            string employeeObjectId = "";

            using (var employeeRepo = new EmployeeRepository())
            {
                employeeObjectId = await employeeRepo.GetEmployeeObjectIdAsync(employee);
            }

            if (StringFunctions.StringIsNullOrEmpty(employeeObjectId))
                throw new System.Exception("El empleado " + EmployeeName + " no existe.");

            return employeeObjectId ;
        }

        public async Task SaveAsync()
        {
            ValidateRequiredData("");
            var employee = await GetEmployeeObjectIdAsync(EmployeeName);

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("SavePetition");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Petition.ObjectID);
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employee);
                    command.Parameters.AddWithValue("pet_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(Petition.Date));
                    command.Parameters.AddWithValue("available_from", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(Petition.AvailableFrom));
                    command.Parameters.AddWithValue("available_to", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(Petition.AvailableTo));

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync()
        {
            ValidateRequiredData("");
            await GetExistingPetitionObjectIdAsync();
            
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("DeletePetition");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Petition.ObjectID);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // TODO create a function to get the objectId of a petition.
        async Task GetExistingPetitionObjectIdAsync()
        {
            var petitions = await GetEmployeePetitionsAsync(EmployeeName);
            Petition.ObjectID = petitions.Where(p => p.Date == Petition.Date).Select(d => d.ObjectID).Single();
        }

        public async Task<List<Petition>> GetEmployeePetitionsAsync(string employeeName)
        {
            var petitions = new List<Petition>();
         
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployeePetitions");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("employee_name", NpgsqlDbType.Text, employeeName);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var objectId = await reader.GetFieldValueAsync<string>(0);
                            var date = await reader.GetFieldValueAsync<System.DateTime>(1);
                            var availableFrom = await reader.GetFieldValueAsync<System.DateTime>(3);
                            var availableTo = await reader.GetFieldValueAsync<System.DateTime>(4);

                            petitions.Add(
                                new Petition
                                {
                                    ObjectID = objectId,
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

        public async Task<bool> CanDoTheTurnAsync(string employeeName, Turn turn)
        {
            bool result = true;

            ValidateRequiredData(employeeName, turn);

            var employeeObjectId = await GetEmployeeObjectIdAsync(employeeName);

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("VerifyAvailability");
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employeeObjectId);
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

        public string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "GetEmployeePetitions":
                    query = "SELECT * FROM get_employee_petitions(:employee_name, :access_key)";
                    break;
                case "SavePetition":
                    query = "SELECT add_petition(:object_id, :employee, :pet_date, :available_from, :available_to)";
                    break;
                case "DeletePetition":
                    query = "SELECT delete_petition(:object_id)";
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
            EmployeeName = null;
            Petition = null;
        }

        public void ValidateRequiredData(string dataToValidate)
        {
            if (StringFunctions.StringIsNullOrEmpty(EmployeeName))
                throw new System. Exception("No se ha indicado empleado");

            if (Petition == null)
                throw new System. NullReferenceException("No se ha indicado petiticion.");

            if (Petition.Date == new NodaTime.LocalDate())
                throw new System. Exception("No se ha indicado fecha en la petition");

            if(StringFunctions.StringIsNullOrEmpty(Petition.ObjectID))
                Petition.ObjectID = StringFunctions.GenerateObjectId(10);
        }

        void ValidateRequiredData(string employeeName, ITurn turn)
        {
            if (StringFunctions.StringIsNullOrEmpty(employeeName))
                throw new System.ArgumentException("No existe el empleado " + employeeName);

            if(turn == null)
                throw new System.NullReferenceException ("No ha indicado turno para verificar disponibilidad.");

            if (turn.Date == new NodaTime.LocalDate())
                throw new System.ArgumentException("No ha indicado fecha para verificar disponibilidad.");
        }
    }
}
