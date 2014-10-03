using System.Threading.Tasks;

namespace MRSES.Core.Entities
{
    public interface ITurnRepository : IDatabase
    {
        Task<Worker[]> GetScheduleAsync(string position, NodaTime.LocalDate ofWeek);
        Task<Worker> GetEmployeeScheduleAsync(IEmployee employee, NodaTime.LocalDate ofWeek);
    }
}
