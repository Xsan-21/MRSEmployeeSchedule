using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using Parse;
using NodaTime;


namespace MRSES.ExternalServices.Parse
{
    public class TurnRepository : ITurnRepository
    {
        #region Fields
        EmployeeRepository _employeeRepository;
        #endregion

        #region Properties

        public IEmployee Employee { private get; set; }

        public ITurn Turn { private get; set; }  

        #endregion

        #region CONSTRUCTORS

        public TurnRepository() 
        {
            _employeeRepository = new EmployeeRepository();
        }

        public TurnRepository(IEmployee employee, ITurn turn)
        {
            Employee = employee;
            Turn = turn;
        }

        #endregion

        #region Methods

        async public Task<Worker[]> GetScheduleAsync(string position, LocalDate ofWeek)
        {
            var allEmployeesOfASpecificPosition = await _employeeRepository.GetAllEmployeesByPosition(position);

            var workerSchedule = await Task.WhenAll(allEmployeesOfASpecificPosition.Select(employee => GetEmployeeScheduleAsync(employee, ofWeek)));

            return workerSchedule;
        }

        async public Task<Worker> GetEmployeeScheduleAsync(IEmployee employee, LocalDate ofWeek)
        {
            var _worker = new Worker(ofWeek, employee.Name);
            var _workerSchedule = _worker.Schedule;    
            var _employeeTurns = await GetEmployeeTurns(employee, ofWeek);

            await FillEmployeeSchedule(_workerSchedule, _employeeTurns);

            return _worker;
        }

        async Task FillEmployeeSchedule(List<Turn> employeeSchedule, IEnumerable<ParseObject> employeeTurns)
        {
            await Task.Run(() => 
            {
                int day = 0;

                foreach (var turn in employeeTurns)
                {
                    var firstTurn = TimeFunctions.GetTurnInAndOut(turn.Get<string>("firstTurn"));
                    var secondTurn = TimeFunctions.GetTurnInAndOut(turn.Get<string>("secondTurn"));

                    employeeSchedule[day].TurnIn1 = firstTurn[0];
                    employeeSchedule[day].TurnOut1 = firstTurn[1];
                    employeeSchedule[day].TurnIn2 = secondTurn[0];
                    employeeSchedule[day].TurnOut2 = secondTurn[1];

                    day++;
                }  
            });                     
        }

        async Task<IEnumerable<ParseObject>> GetEmployeeTurns(IEmployee employee, LocalDate ofWeek)
        {
            var _employee = await GetEmployeeParseObject(employee);

            var _employeeTurns = from turns in ParseObject.GetQuery("Turns")
                                 where turns.Get<ParseObject>("employee") == _employee
                                 where turns.Get<DateTime>("ofWeek") == DateFunctions.FromLocalDateToDateTime(ofWeek)
                                 orderby turns.Get<DateTime>("date")
                                 select turns;

            return await _employeeTurns.FindAsync();
        }

        async public Task SaveAsync()
        {
            ValidateEmployeeAndTurn();
            bool turnExists = await ExistsAsync();

            if (turnExists)
                await UpdateAsync();
            else
                await SaveNewTurn();            
        }

        private async Task SaveNewTurn()
        {
            ParseObject employee = await GetEmployeeParseObject(Employee);
            var newTurn = new ParseObject("Turns") 
                   { 
                       { "ofWeek", DateFunctions.FromLocalDateToDateTime(Turn.OfWeek) },
                       {"date", DateFunctions.FromLocalDateToDateTime(Turn.Date)},
                       {"firstTurn", Turn.FirstTurn},
                       {"secondTurn", Turn.SecondTurn},
                       {"totalHours", Turn.Hours},
                       {"employee", employee}
                   };

            await newTurn.SaveAsync();
        }

        async public Task DeleteAsync()
        {
            var turn = await GetTurnIfExists();

            if (turn != null)
                await turn.DeleteAsync();
        }

        async public Task<bool> ExistsAsync()
        {
            ParseObject employee = await GetEmployeeParseObject(Employee);

            var turn = await GetTurnIfExists();

            return turn == null ? false : true;
        }

        async Task UpdateAsync()
        {
            var existingTurn = await GetTurnIfExists();
            existingTurn["firstTurn"] = Turn.FirstTurn;
            existingTurn["secondTurn"] = Turn.SecondTurn;
            existingTurn["totalHours"] = Turn.Hours;

            await existingTurn.SaveAsync();
        }

        async Task<ParseObject> GetTurnIfExists()
        {
            ParseObject employee = await GetEmployeeParseObject(Employee);

            var query = from turns in ParseObject.GetQuery("Turns")
                        where turns.Get<ParseObject>("employee") == employee
                        where turns.Get<DateTime>("date") == DateFunctions.FromLocalDateToDateTime(Turn.Date)
                        select turns;

            return await query.FirstOrDefaultAsync();
        }

        async Task<ParseObject> GetEmployeeParseObject(IEmployee employee)
        {
            return await EmployeeRepository.GetEmployeeObjectWithoutData(employee);
        }

        void ValidateEmployeeAndTurn()
        {
            if (Employee == null || Turn == null)
                throw new NullReferenceException("No se ha especificado empleado o turno a guardar.");
        }

        #endregion        
    }
}
