namespace MRSES.Core.Entities
{
    public interface IAvailabilityRepository
    {
        System.Threading.Tasks.Task SaveAsync();
        System.Threading.Tasks.Task<Availability> GetAvailabilityAsync();
        System.Threading.Tasks.Task<string> GetAvailabilityOfADayAsync(NodaTime.IsoDayOfWeek dayOfWeek);
        System.Threading.Tasks.Task<bool> CanDoTheTurnAsync(IEmployee employee, ITurn turn);
    }
}
