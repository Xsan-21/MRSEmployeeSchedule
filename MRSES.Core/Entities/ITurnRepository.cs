using System.Threading.Tasks;

namespace MRSES.Core.Entities
{
    public interface ITurnRepository : IDatabase
    {
        Task<Schedule[]> GetScheduleAsync(string position, NodaTime.LocalDate ofWeek);
        Task<Schedule> GetEmployeeScheduleAsync(IEmployee employee, NodaTime.LocalDate ofWeek);
    }
}
