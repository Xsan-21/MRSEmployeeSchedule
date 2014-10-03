using System.Threading.Tasks;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using Parse;

namespace MRSES.ExternalServices.Parse
{
    class AvailabilityRepository : System.IDisposable, IAvailabilityRepository, IDatabase
    {
        #region Fields

        private Availability _availability;
        private IEmployee _employee; 

        #endregion

        #region Properties

        public Availability Availability
        {
            private get { return _availability; }
            set { _availability = value; }
        }

        public IEmployee Employee
        {
            private get { return _employee; }
            set { _employee = value; }
        } 

        #endregion

        #region Constructors

        public AvailabilityRepository() { }

        public AvailabilityRepository(IEmployee employee, Availability availability)
        {
            Employee = employee;
            Availability = availability;
        } 

        #endregion       

        #region Methods

        public async Task SaveAsync()
        {
            ValidateEmployeeAndAvailability();

            if(await ExistsAsync()) 
            {
                await UpdateAsync();
            }
            else
            {
                await  SaveAsNewAvailabilityAsync();  
            }                          

            Dispose();
        }

        private async Task<ParseObject> SearchAvailabilityAsync(IEmployee employee)
        {
            var _employee = await EmployeeRepository.GetEmployeeObjectWithoutData(employee);

            if (_employee == null)
                throw new System.Exception("The requested employee does not exists");

            return await (from employeeAvailability in ParseObject.GetQuery("Availability")
                          where employeeAvailability.Get<ParseObject>("employee") == _employee
                          select employeeAvailability).FirstOrDefaultAsync();            
        }

        private async Task SaveAsNewAvailabilityAsync()
        {
            ParseObject employee = await EmployeeRepository.GetEmployeeObjectWithoutData(Employee);
            var availability = new ParseObject("Availability")
		    {
			    {"monday", Availability.Monday},
			    {"tuesday", Availability.Tuesday},
			    {"wednesday", Availability.Wednesday},
			    {"thursday", Availability.Thursday},
			    {"friday", Availability.Friday},
			    {"saturday", Availability.Saturday},
			    {"sunday", Availability.Sunday}
		    };

            availability["employee"] = employee;

            await availability.SaveAsync();
        }

        public async Task<bool> CanDoTheTurnAsync(IEmployee employee, ITurn turn)
        {
            return await VerifyIfEmployeeCanDoTheTurn(employee, turn);
        }

        private async Task<bool> VerifyIfEmployeeCanDoTheTurn(IEmployee employee, ITurn turn)
        {
            if (!turn.IsFreeDay)
            {
                var dayAvailability = await GetEmployeeAvailabilityOfADayAsync(employee, turn.Date.IsoDayOfWeek);

                if (string.IsNullOrEmpty(dayAvailability))
                    return true;
                else if (string.Compare(dayAvailability, "NO DISPONIBLE", true) == 0)
                    return false;

                var availability = TimeFunctions.GetTurnInAndOut(dayAvailability);

                return VerifyFirstTurn(turn, availability) & VerifySecondTurn(turn, availability); 
            }

            return true;
        }

        public async Task<Availability> GetEmployeeAvailabilityAsync(IEmployee employee)
        {
            var searchResult = await SearchAvailabilityAsync(employee);
            var availability = new Availability();

            if (searchResult != null)
            {

                availability.Friday = searchResult.Get<string>("friday");
                availability.Saturday = searchResult.Get<string>("saturday");
                availability.Sunday = searchResult.Get<string>("sunday");
                availability.Monday = searchResult.Get<string>("monday");
                availability.Tuesday = searchResult.Get<string>("tuesday");
                availability.Wednesday = searchResult.Get<string>("wednesday");
                availability.Thursday = searchResult.Get<string>("thursday");
            }

            return availability;
        }

        private async Task<string> GetEmployeeAvailabilityOfADayAsync(IEmployee employee, NodaTime.IsoDayOfWeek dayOfWeek)
        {
            string result = string.Empty;
            var employeeAvailability = await GetEmployeeAvailabilityAsync(employee);

            switch (dayOfWeek)
            {
                case NodaTime.IsoDayOfWeek.Friday:
                    result = employeeAvailability.Friday;
                    break;
                case NodaTime.IsoDayOfWeek.Monday:
                    result = employeeAvailability.Monday;
                    break;
                case NodaTime.IsoDayOfWeek.Saturday:
                    result = employeeAvailability.Saturday;
                    break;
                case NodaTime.IsoDayOfWeek.Sunday:
                    result = employeeAvailability.Sunday;
                    break;
                case NodaTime.IsoDayOfWeek.Thursday:
                    result = employeeAvailability.Thursday;
                    break;
                case NodaTime.IsoDayOfWeek.Tuesday:
                    result = employeeAvailability.Tuesday;
                    break;
                case NodaTime.IsoDayOfWeek.Wednesday:
                    result = employeeAvailability.Wednesday;
                    break;
                default:
                    break;
            }

            return result;
        }

        private bool VerifyFirstTurn(ITurn turn, NodaTime.LocalTime[] availability)
        {
            bool result = true;

            if (!turn.FirstTurn.Contains("X"))
            {
                if (turn.TurnIn1 < availability[0])
                    result =  false;

                if (turn.TurnOut1 > availability[1])
                    result = false;
            }

            return result;            
        }

        private bool VerifySecondTurn(ITurn turn, NodaTime.LocalTime[] availability)
        {
            bool result = true;

            if (!turn.SecondTurn.Contains("X"))
            {
                if (turn.TurnIn2 < availability[0])
                    result = false;

                if (turn.TurnOut2 > availability[1])
                    result = false;
            }

            return result;
        }

        public void Dispose()
        {
            Employee = null;
            Availability = null;
        }

        private void ValidateEmployeeAndAvailability()
        {
            if (Employee == null || Availability == null)
                throw new System.Exception("No se ha especificado empleado o disponibilidad");
        }

        public async Task DeleteAsync()
        {
            ValidateEmployeeAndAvailability();
            var availability = await SearchAvailabilityAsync(Employee);
            await availability.DeleteAsync();
        }

        public async Task<bool> ExistsAsync()
        {
            ValidateEmployeeAndAvailability();
            ParseObject employee = await EmployeeRepository.GetEmployeeObjectWithoutData(Employee);
            var query = await(from employeeAvailability in ParseObject.GetQuery("Availability")
                         where employeeAvailability["employee"] == employee
                         select employeeAvailability).FirstOrDefaultAsync();

            return query == null ? false : true;
        }

        async Task UpdateAsync()
        {
            ValidateEmployeeAndAvailability();
            var availability = await SearchAvailabilityAsync(Employee);

            availability["monday"] = Availability.Monday;
            availability["tuesday"] = Availability.Tuesday;
            availability["wednesday"] = Availability.Wednesday;
            availability["thursday"] = Availability.Thursday;
            availability["friday"] = Availability.Friday;
            availability["saturday"] = Availability.Saturday;
            availability["sunday"] = Availability.Sunday;

            await availability.SaveAsync();
        }

        #endregion
    }
}
