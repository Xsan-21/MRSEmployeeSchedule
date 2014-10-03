using System.Threading.Tasks;

namespace MRSES.Core.Entities
{
    public interface IPetitionRepository : IAvailabilityRepository
    {
        Task<Petition[]> GetAllPetitions(NodaTime.LocalDate ofWeek, string position);
    }
}
