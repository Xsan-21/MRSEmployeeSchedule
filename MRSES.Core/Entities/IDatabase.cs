namespace MRSES.Core.Entities
{
    public interface IDatabase : System.IDisposable
    {
        System.Threading.Tasks.Task SaveAsync();
    }
}
