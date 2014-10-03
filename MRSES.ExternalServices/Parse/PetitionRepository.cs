using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using MRSES.Core;
using NodaTime;

namespace MRSES.ExternalServices.Parse
{
    class PetitionRepository : IDisposable, IPetitionRepository
    {
        #region Variables

        IPetition _petition;

        #endregion 
        
        #region Properties

        public IPetition Petition
        {
            set { _petition = value; }
            private get { return _petition; }
        }

        #endregion 

        #region Constructors

        public PetitionRepository() { }

        public PetitionRepository(IPetition petition)
        {
            Petition = petition;
        }

        #endregion 

        #region Methods
        public async Task SaveAsync()
        {
            ValidatePetitionAndEmployee();

            var employee = await EmployeeRepository.GetEmployeeObjectWithoutData(Petition.Employee);
            var _petition = new ParseObject("Petitions")
                {
                    {"date", DateFunctions.FromLocalDateToDateTime(Petition.Date)},
                    {"availableFrom", DateFunctions.ConvertLocalDateTimeToDateTime(Petition.AvailableFrom)},
                    {"availableTo", DateFunctions.ConvertLocalDateTimeToDateTime(Petition.AvailableTo)},
                    {"freeDay", Petition.FreeDay}
                };

            _petition["employee"] = employee;
            await _petition.SaveAsync();

            Dispose();
        }

        public async Task DeleteAsync()
        {
            ValidatePetitionAndEmployee();
            if (!await ExistsAsync(Petition.Employee, Petition.Date))
                return;

            var existingPetition = await GetPetitionIfExists(Petition.Employee, Petition.Date);
            await existingPetition.DeleteAsync();
        }

        public async Task<bool> CanDoTheTurnAsync(IEmployee employee, ITurn turnToBeSave)
        {
            try
            {
                return await VerifyIfEmployeeCanDoTheTurn(employee, turnToBeSave);
            }
            catch (NullReferenceException)
            {
                return true;
            }            
        }

        public async Task<Petition[]> GetAllPetitions(LocalDate ofWeek, string position)
        { 
            var petitions = await RetriveAllPetitionsAsync(ofWeek, position);

            return await Task.WhenAll(petitions.Select(petition => PrepareEmployeePetition(petition)));
        }

        private async Task<IEnumerable<ParseObject>> RetriveAllPetitionsAsync(LocalDate ofWeek, string position)
        {
            var employees = from employee in ParseObject.GetQuery("Employees")
                            where employee.Get<string>("store") == Configuration.StoreLocation
                            where employee.Get<string>("position") == position
                            select employee;

            var petitions = from petition in ParseObject.GetQuery("Petitions")
                            where petition.Get<DateTime>("date") >= DateFunctions.FromLocalDateToDateTime(ofWeek)
                            orderby petition.Get<DateTime>("date") ascending
                            join employee in employees on petition["employee"] equals employee
                            select petition;

            return await petitions.FindAsync();
        }

        private async Task<bool> VerifyIfEmployeeCanDoTheTurn(IEmployee employee, ITurn turnToBeSave)
        {
            var existingPetition = await GetEmployeePetitionAsync(employee, turnToBeSave.Date);

            if (existingPetition.FreeDay)
                return false;

            return VerifyFirstTurn(turnToBeSave, existingPetition) & VerifySecondTurn(turnToBeSave, existingPetition);
        }

        private bool VerifyFirstTurn(ITurn turn, IPetition petition)
        {
            bool result = true;

            if (!turn.FirstTurn.Contains("X"))
            {
                if (turn.TurnIn1 < petition.AvailableFrom.TimeOfDay)
                    result = false;
                if (turn.TurnOut1 > petition.AvailableTo.TimeOfDay)
                    result = false;
            }

            return result;
        }

        private bool VerifySecondTurn(ITurn turn, IPetition petition)
        {
            bool result = true;

            if (!turn.SecondTurn.Contains("X"))
            {
                if (turn.TurnIn2 < petition.AvailableFrom.TimeOfDay)
                    result = false;
                if (turn.TurnOut2 > petition.AvailableTo.TimeOfDay)
                    result = false;
            }

            return result;
        }

        public async Task<Petition> GetEmployeePetitionAsync(IEmployee employee, LocalDate date)
        {
            var petition = await GetPetitionIfExists(employee, date);

            if (petition == null)
                throw new NullReferenceException("No hay peticion del empleado para el dia indicado");

            return await PrepareEmployeePetition(petition);       
        }        
        
        private async Task<bool> ExistsAsync(IEmployee employee, LocalDate date)
        {
            var petition = await GetPetitionIfExists(employee, date);

            return petition == null ? false : true;
        }

        private async Task<ParseObject> GetPetitionIfExists(IEmployee employee, LocalDate date)
        {
            ParseObject _employee = await EmployeeRepository.GetEmployeeObjectWithoutData(employee);

            if (_employee == null)
                throw new Exception("The requested employee does not exists");

            var petition = from petitions in ParseObject.GetQuery("Petitions")
                           where petitions.Get<ParseObject>("employee") == _employee
                           where petitions.Get<DateTime>("date") == DateFunctions.FromLocalDateToDateTime(date)
                           select petitions;

            return await petition.FirstOrDefaultAsync();
        }

        private async Task<Petition> PrepareEmployeePetition(ParseObject petition)
        {
            return await Task.Run(async () =>
            {
                var employee = await petition.Get<ParseObject>("employee").FetchIfNeededAsync();
                
                return new Petition()
                {
                    Employee = await EmployeeRepository.CreateEmployee(employee),
                    AvailableFrom = DateFunctions.FromDateTimeToLocalDateTime(petition.Get<DateTime>("availableFrom")),
                    AvailableTo = DateFunctions.FromDateTimeToLocalDateTime(petition.Get<DateTime>("availableTo")),
                    Date = DateFunctions.FromDateTimeToLocalDate(petition.Get<DateTime>("date"))
                };
            });
        }

        private void ValidatePetitionAndEmployee()
        {
             if (Petition == null)
                throw new Exception("No se ha especificado empleado o fecha de petición");
        }

        public void Dispose()
        {
            Petition = null;
        }

        #endregion        
    }
}
