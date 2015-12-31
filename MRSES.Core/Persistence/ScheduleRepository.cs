using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MRSES.Core.Entities;
using NpgsqlTypes;
using Npgsql;
using MRSES.Core.Shared;

namespace MRSES.Core.Persistence
{
    public interface IScheduleRepository : IDatabase
    {
        Task<Schedule[]> GetScheduleByPositionAsync(string position, NodaTime.LocalDate ofWeek);
        Task<Schedule> GetEmployeeScheduleAsync(string employeeName, NodaTime.LocalDate ofWeek);
        Task<Turn> GetEmployeeTurnAsync(string employeeName, NodaTime.LocalDate date);
        Task<Dictionary<string, Turn>> ShowTurnsByDay(string position, NodaTime.LocalDate ofWeek, NodaTime.LocalDate date);
        Task<bool> ViolateCycleOfTwentyFourHour(string employeeName, ITurn turn);
        List<Schedule> Schedule { get; set; }
    }

    public class ScheduleRepository : IScheduleRepository
    {
        IEmployeeRepository _employeeRepository;
        public List<Schedule> Schedule { get; set; }

        public ScheduleRepository()
        {
            _employeeRepository = new EmployeeRepository();
            Schedule = new List<Schedule>();
        }

        public ScheduleRepository(List<Schedule> schedule)
        {
            Schedule = schedule;
        }

        async public Task SaveAsync()
        {
            ValidateRequiredDataForAction();

            foreach (var schedule in Schedule)
            {
                await SaveScheduleAsync(schedule);
            }
        }

        async Task<string> GetEmployeeObjectIdAsync(string employeeName)
        {
            var employeeObjectId = await EmployeeRepository.GetEmployeeObjectIdAsync(employeeName);

            if (StringFunctions.StringIsNullOrEmpty(employeeObjectId))
                throw new Exception("El empleado " + employeeName + " no existe.");

            return employeeObjectId;
        }

        async Task SaveScheduleAsync(Schedule schedule)
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                var employeeObjectId = await GetEmployeeObjectIdAsync(schedule.Employee);

                await dbConnection.OpenAsync();

                foreach (var turn in schedule.WeekDays)
                {
                    var turnObjectId = await GetTurnObjectIdAsync(turn, employeeObjectId);

                    using (var command = new NpgsqlCommand("", dbConnection))
                    {
                        command.CommandText = GetQuery("Save");
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, turnObjectId);
                        command.Parameters.AddWithValue("employee", NpgsqlDbType.Text, employeeObjectId);
                        command.Parameters.AddWithValue("turn_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(turn.Date));
                        command.Parameters.AddWithValue("of_week", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(schedule.OfWeek));
                        command.Parameters.AddWithValue("turn", NpgsqlDbType.Array | NpgsqlDbType.Time, turn.GetTurn());

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        string GetQuery(string action)
        {
            string query = "";
            switch (action)
            {
                case "Save":
                    query = "SELECT add_turn(:object_id, :employee, :of_week, :turn_date, :turn)";
                    break;
                case "GetEmployeeSchedule":
                    query = "SELECT * FROM get_employee_schedule(:employee_object_id, :of_week, :access_key)";
                    break;
                case "GetSingleDaySchedule":
                    query = "SELECT * FROM get_schedule_by_day(:position, :store, :date)";
                    break;
                case "Verify24HourCycle":
                    query = "SELECT violate_cycle_of_24_hours(:employee_name, :access_key, :turn_date, :first_turn)";
                    break;
                case "GetTurnObjectID":
                    query = "select get_turn_object_id(:employee_object_id, :access_key, :turn_date)";
                    break;
                default:
                    break;
            }

            return query;
        }

        public async Task<List<TurnToSync>> SyncTurnsDataAsync(DateTime lastSyncDate)
        {
            var turns = new List<TurnToSync>();
 
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "SELECT * FROM sync_schedule(:sync_date, :access_key)";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("sync_date", NpgsqlDbType.Timestamp, lastSyncDate);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var objectId = await reader.GetFieldValueAsync<string>(0);
                            var employee = await reader.GetFieldValueAsync<string>(1);
                            var turnDate = await reader.GetFieldValueAsync<DateTime>(2);
                            var turn = await reader.GetFieldValueAsync<NpgsqlTime[]>(3);

                            turns.Add(new TurnToSync
                            {
                                Employee = employee,
                                Turn = new Turn(DateFunctions.FromDateTimeToLocalDate(turnDate))
                                {
                                    ObjectId = objectId,
                                    TurnHours = new NodaTime.LocalTime[]
                                    {
                                        new NodaTime.LocalTime(turn[0].Hours, turn[0].Minutes),
                                        new NodaTime.LocalTime(turn[1].Hours, turn[1].Minutes),
                                        new NodaTime.LocalTime(turn[2].Hours, turn[2].Minutes),
                                        new NodaTime.LocalTime(turn[3].Hours, turn[3].Minutes)
                                    }
                                }
                            });  
                        }
                    }
                }
            }

            return turns;
        }

        async public Task<Dictionary<string, Turn>> ShowTurnsByDay(string position, NodaTime.LocalDate ofWeek, NodaTime.LocalDate date)
        {
            if (date < ofWeek || date > ofWeek.PlusDays(6))
                throw new Exception("La fecha indicada no concuerda con la semana");

            var result = new Dictionary<string, Turn>();

            var schedule = await GetScheduleByPositionAsync(position, ofWeek);

            var scheduleByDay = from sch in schedule
                                from day in sch.WeekDays
                                where day.Date == date
                                select new { Name = sch.Employee, Turn = day };

            foreach (var turn in scheduleByDay)
            {
                result.Add(turn.Name, turn.Turn);
            }

            return result;
        }

        public async Task<Schedule[]> GetScheduleByPositionAsync(string position, NodaTime.LocalDate ofWeek)
        {
            var employeeNamesByPosition = await _employeeRepository.GetEmployeeNamesByPositionAsync(position);

            if (employeeNamesByPosition.Count == 0)
                throw new Exception("No existen empleados de la posición indicada.");

            return await Task.WhenAll(employeeNamesByPosition.Select(async employeeName => await GetEmployeeScheduleAsync(employeeName, ofWeek)));
        }

        public async Task<Schedule> GetEmployeeScheduleAsync(string employeeName, NodaTime.LocalDate ofWeek)
        {
            if (StringFunctions.StringIsNullOrEmpty(employeeName) || ofWeek == null)
                throw new Exception("No se ha indicado nombre de empleado o semana");

            var emp_schedule = new Schedule(employeeName, ofWeek);
            var employeeObjectId = await GetEmployeeObjectIdAsync(employeeName);
            int counter = 0;

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetEmployeeSchedule");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("employee_object_id", NpgsqlDbType.Text, employeeObjectId);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);
                    command.Parameters.AddWithValue("of_week", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(ofWeek));

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!(await reader.ReadAsync()))
                            return emp_schedule;

                        do
                        {
                            var objecId = await reader.GetFieldValueAsync<string>(0);
                            var turn_date = await reader.GetFieldValueAsync<DateTime>(1);
                            var turn_hours = await reader.GetFieldValueAsync<NpgsqlTime[]>(2);

                            emp_schedule.WeekDays[counter].ObjectId = objecId;
                            emp_schedule.WeekDays[counter].Date = DateFunctions.FromDateTimeToLocalDate(turn_date);
                            emp_schedule.WeekDays[counter].TurnHours = turn_hours.Select(t => new NodaTime.LocalTime(t.Hours, t.Minutes)).ToArray();

                            ++counter;
                        } while (await reader.ReadAsync());
                    }
                }
            }

            return emp_schedule;
        }

        public async Task<Turn> GetEmployeeTurnAsync(string employeeName, NodaTime.LocalDate date)
        {
            var weekOfDate = DateFunctions.GetWeekOf(date);
            var employeeSchedule = await GetEmployeeScheduleAsync(employeeName, weekOfDate);
            return employeeSchedule.WeekDays.Where(t => t.Date == date).Single();
        }

        async Task<string> GetTurnObjectIdAsync(Turn turn, string employeeObjectId)
        {
            var turnObjectId = "";

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetTurnObjectID");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("employee_object_id", NpgsqlDbType.Text, employeeObjectId);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);
                    command.Parameters.AddWithValue("turn_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(turn.Date));

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            turnObjectId = await reader.GetFieldValueAsync<string>(0);
                    }
                }
            }

            return StringFunctions.StringIsNullOrEmpty(turnObjectId) ? StringFunctions.GenerateObjectId(10) : turnObjectId;
        }

        public async Task<bool> ViolateCycleOfTwentyFourHour(string employeeName, ITurn turn)
        {
            bool result = true;

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("Verify24HourCycle");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("employee_name", NpgsqlDbType.Text, employeeName);
                    command.Parameters.AddWithValue("turn_date", NpgsqlDbType.Date, DateFunctions.FromLocalDateToDateTime(turn.Date));
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    var firstTurn = new DateTime[] 
                    {
                        DateFunctions.FromLocalTimeToDateTime(turn.TurnHours[0]),
                        DateFunctions.FromLocalTimeToDateTime(turn.TurnHours[1])
                    };


                    command.Parameters.AddWithValue("first_turn", NpgsqlDbType.Array | NpgsqlDbType.Time, firstTurn);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                        if (await reader.ReadAsync())
                            result = await reader.GetFieldValueAsync<bool>(0);
         
                }
            }

            return result;
        }

        public void ValidateRequiredDataForAction()
        {
            if (Schedule == null)
                throw new Exception("No se ha establecido horario a guardar.");

            if (Schedule.Any(e => StringFunctions.StringIsNullOrEmpty(e.Employee)))
                throw new Exception("No se ha indicado empleado.");
        }

        public void Dispose()
        {
            Schedule = null;
        }
    }
}
