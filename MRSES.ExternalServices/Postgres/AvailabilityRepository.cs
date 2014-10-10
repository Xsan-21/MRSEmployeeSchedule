using MRSES.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSES.ExternalServices.Postgres
{
    public class AvailabilityRepository// : System.IDisposable, IAvailabilityRepository, IDatabase
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
    }
}
