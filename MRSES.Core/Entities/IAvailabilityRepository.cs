namespace MRSES.Core.Entities
{
    public interface IAvailabilityRepository : IAvailable
    {
        System.Threading.Tasks.Task SaveAsync();
        System.Threading.Tasks.Task<Availability> GetAvailabilityAsync();
        System.Threading.Tasks.Task<string> GetAvailabilityOfADayAsync(NodaTime.IsoDayOfWeek dayOfWeek);
    }
}
