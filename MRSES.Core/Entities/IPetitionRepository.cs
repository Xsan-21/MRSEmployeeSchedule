namespace MRSES.Core.Entities
{
    public interface IPetitionRepository : IAvailable
    {
        System.Threading.Tasks.Task<Petition[]> GetAllPetitions(NodaTime.LocalDate ofWeek, string position);
    }
}
