namespace MRSES.Core.Entities
{
    public interface IDatabase : System.IDisposable
    {
        System.Threading.Tasks.Task SaveAsync();
        void ValidateRequiredData(string dataToValidate);
        string GetQuery(string action);
    }
}
