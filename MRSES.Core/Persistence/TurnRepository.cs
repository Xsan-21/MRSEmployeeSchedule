using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MRSES.Core.Entities;
using NpgsqlTypes;
using Npgsql;
using MRSES.Core.Shared;

namespace MRSES.Core.Persistence
{
    public class TurnRepository : ITurnRepository, IDisposable
    {
        public Schedule Schedule { get; set; }

        public TurnRepository()
        {

        }

        public TurnRepository(Schedule schedule)
        {
            Schedule = schedule;
        }

        async public Task SaveAsync()
        {
            if (Schedule == null)
                throw new Exception("No se ha establecido horario a guardar.");

            await Task.Run(() => Schedule.Turns.ForEach(async turn => await SaveScheduleAsync(turn)));

            Dispose();
        }

        async Task SaveScheduleAsync(Turn turn)
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.PostgresDbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Save");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("emp_name", NpgsqlDbType.Varchar, Schedule.Name);
                    command.Parameters.AddWithValue("emp_store", NpgsqlDbType.Varchar, Configuration.StoreLocation);
                    command.Parameters.AddWithValue("turn_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(turn.Date));
                    command.Parameters.AddWithValue("first_turn_in", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(turn.TurnIn1));
                    command.Parameters.AddWithValue("first_turn_out", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(turn.TurnOut1));
                    command.Parameters.AddWithValue("second_turn_in", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(turn.TurnIn2));
                    command.Parameters.AddWithValue("second_turn_out", NpgsqlDbType.Time, DateFunctions.FromLocalTimeToDateTime(turn.TurnOut2));
                    command.Parameters.AddWithValue("of_week", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(Schedule.OfWeek));

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        string GetQuery(string action)
        {
            string query = "";
            switch (action)
            {
                case "Save":
                    query = "SELECT add_turn(:emp_name, :emp_store, :turn_date, :first_turn_in, :first_turn_out, :second_turn_in, :second_turn_out, :of_week)";
                    break;
                default:
                    break;
            }
            return query;
        }

        public void Dispose()
        {
            Schedule = null;
        }

        public Task<Schedule[]> GetScheduleByPositionAsync(string position, NodaTime.LocalDate ofWeek)
        {
            throw new NotImplementedException();
        }

        public Task<Schedule> GetEmployeeScheduleAsync(IEmployee employee, NodaTime.LocalDate ofWeek)
        {
            throw new NotImplementedException();
        }
    }
}
