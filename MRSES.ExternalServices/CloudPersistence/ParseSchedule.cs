using System.Linq;
using System.Threading.Tasks;
using Parse;
using MRSES.Core.Entities;
using MRSES.Core.Persistence;

namespace MRSES.ExternalServices.CloudPersistence
{
    internal class ParseSchedule
    {
        internal async Task SyncAsync()
        {
            await UpdateOrSaveTurnAsync();
        }

        async Task UpdateOrSaveTurnAsync()
        {
            var turns = await GetTurns();

            if (!turns.All(t => string.IsNullOrEmpty(t.Turn.ObjectId)))
            {
                foreach (var turn in turns)
                {
                    var update = await UpdateTurnAsync(turn);
                    if (!update)
                    {
                        await SaveScheduleAsync(turn);
                    }
                } 
            }
        }

        async Task SaveScheduleAsync(TurnToSync turn)
        {
            try
            {
                var _turn = new[] {
                    turn.Turn.TurnHours[0].ToString(),
                    turn.Turn.TurnHours[1].ToString(),
                    turn.Turn.TurnHours[2].ToString(),
                    turn.Turn.TurnHours[3].ToString()
                };

                using (var _employeeRepo = new EmployeeRepository())
                {
                    var employee = await _employeeRepo.GetEmployeeAsync(turn.Employee);
                    var newTurn = new ParseObject("Schedule")
                    {
                        { "postgresId", turn.Turn.ObjectId },
                        { "employee", turn.Employee },
                        { "location", employee.Location },
                        { "turn_date", Core.Shared.DateFunctions.FromLocalDateToDateTime(turn.Turn.Date) },
                        { "turn", _turn },
                        { "turn_hours", turn.Turn.Hours }
                    };

                    await newTurn.SaveAsync(); 
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                throw new System.Exception("No se pudo establecer conexión a Internet.");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        async Task<bool> UpdateTurnAsync(TurnToSync turn)
        {
            try
            {
                var existingTurn = await GetTurnObject(turn.Turn.ObjectId);

                if (existingTurn == null)
                {
                    return false;
                }

                var _turn = new[] {
                    turn.Turn.TurnHours[0].ToString(),
                    turn.Turn.TurnHours[1].ToString(),
                    turn.Turn.TurnHours[2].ToString(),
                    turn.Turn.TurnHours[3].ToString()
                };

                existingTurn["turn_hours"] = turn.Turn.Hours;
                existingTurn["turn"] = _turn;

                await existingTurn.SaveAsync();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                throw new System.Exception("No se pudo establecer conexión a Internet.");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return true;
        }

        async Task<System.Collections.Generic.List<TurnToSync>> GetTurns()
        {
            using (var scheduleRepo = new ScheduleRepository())
            {
                return await scheduleRepo.SyncTurnsDataAsync(Configuration.LastSyncDate);
            }
        }

        async Task<ParseObject> GetTurnObject(string postgresId)
        {
            var _object = new ParseObject("Schedule");

            try
            {
                _object = await ParseObject.GetQuery("Schedule").WhereEqualTo("postgresId", postgresId).FirstOrDefaultAsync();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                throw new System.Exception("No se pudo establecer conexión a Internet.");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return _object;
        }
    }
}
