using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRSES.Core.Entities
{
    public interface IEmployeeRepository : IDatabase
    {
        Task<Employee> GetEmployeeAsync(string name_or_id);
        Task<List<string>> GetPositionsAsync();
        Task<List<string>> GetEmployeeNamesByPositionAsync(string position);
        //Task<List<string>> GetAllEmployeeNamesWithId();  TODO delete if not needed 
        //Task<List<Employee>> GetAllEmployeesByPositionAsync(string position); TODO delete if not needed
    }
}
