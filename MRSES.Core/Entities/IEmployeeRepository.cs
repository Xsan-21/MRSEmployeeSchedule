using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRSES.Core.Entities
{
    public interface IEmployeeRepository : IDatabase
    {
        Task<Employee> GetEmployee(string name, int id);
        Task<List<string>> GetPositions();
        Task<List<string>> GetAllEmployeeNamesWithId();
        Task<List<Employee>> GetAllEmployeesByPosition(string position);
    }
}
