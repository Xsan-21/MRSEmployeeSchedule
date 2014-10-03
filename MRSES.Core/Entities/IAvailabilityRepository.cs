namespace MRSES.Core.Entities
{
    public interface IAvailabilityRepository
    {
        System.Threading.Tasks.Task<bool> CanDoTheTurnAsync(IEmployee employee, ITurn turn);
    }
}
