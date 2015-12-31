using System.Threading.Tasks;
using MRSES.ExternalServices.CloudPersistence;
using Parse;

namespace MRSES.ExternalServices
{
    public class DataSynchronization
    {
        Core.Persistence.DeleteDataRepository deleteDataRepo;
        System.DateTime currentLastSyncDate;

        public DataSynchronization()
        {
            currentLastSyncDate = System.DateTime.Now;
            deleteDataRepo = new Core.Persistence.DeleteDataRepository();
            
            // Interesting, I learned here that if we put an array of Tasks in a contructor, 
            // it will run the asynchronous tasks without the need of declaring the constructor async!

            // This declaration will run all the tasks in the array without blocking the thread!
            //dataToSave = new[] {
            //new ParseBusiness().SyncAsync(),
            //};
        }

        public async Task SyncDataAsync()
        {
            await SyncDeleteDataAsync();

            await Task.WhenAll(new Task[] {
               //new ParseBusiness().SyncAsync(),
               new ParseLocation().SyncAsync(),
               new ParseEmployee().SyncAsync(),
               new ParsePetition().SyncAsync(),
               new ParseSchedule().SyncAsync()
            });

            Configuration.LastSyncDate = currentLastSyncDate;
        }

        async Task SyncDeleteDataAsync()
        {
            var objectsToDeleteInParseData = await deleteDataRepo.GetDeletedObjectsAsync(Configuration.LastSyncDate);

            // where deletedObject.Value = class name and deletedObject.Key = objectId of the record.
            foreach (var deletedObject in objectsToDeleteInParseData)
            {
                var query = await  ParseObject.GetQuery(deletedObject.Value).WhereEqualTo("postgresId", deletedObject.Key).FirstOrDefaultAsync();

                if(query != null)
                    await query.DeleteAsync();
            }

            await deleteDataRepo.DeleteObjectsAsync(currentLastSyncDate);

            // TODO get deleted petitions by users from Parse and deleted those petitions in postgres database.
        }
    }
}
