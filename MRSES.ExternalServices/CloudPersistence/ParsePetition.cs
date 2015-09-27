using System;
using System.Linq;
using System.Threading.Tasks;
using Parse;
using MRSES.Core.Entities;
using MRSES.Core.Persistence;
using MRSES.Core.Shared;
using System.Net.Http;

namespace MRSES.ExternalServices.CloudPersistence
{
    internal class ParsePetition
    {
        IEmployeeRepository _employeeRepo;

        public async Task SyncAsync()
        {
            var petitions = await GetPetitions();

            await RetrivePetitionsFromParse();

            if (!petitions.All(p => string.IsNullOrEmpty(p.ObjectId)))
                foreach (var petition in petitions)
                    await SavePetitionAsync(petition);
        }

        async Task SavePetitionAsync(IPetition petition)
        {
            try
            {
                var date = DateFunctions.FromLocalDateToDateTime(petition.Date);
                var availableFrom = DateFunctions.FromLocalTimeToDateTime(petition.AvailableFrom);
                var availableTo = DateFunctions.FromLocalTimeToDateTime(petition.AvailableTo);

                using (_employeeRepo = new EmployeeRepository())
                {
                    var employee = await _employeeRepo.GetEmployeeAsync(petition.Employee);

                    var newPetition = new ParseObject("Petition")
                    {
                        { "postgresId", petition.ObjectId },
                        { "employee", petition.Employee },
                        { "location",  employee.Location},
                        { "date", date },
                        { "freeDay", petition.FreeDay },
                        { "availableFrom", availableFrom },
                        { "availableTo", availableTo }
                    };

                    await newPetition.SaveAsync(); 
                }
            }
            catch (HttpRequestException)
            {
                throw new Exception("No se pudo establecer conexión a Internet.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal async Task<System.Collections.Generic.List<Petition>> GetPetitions()
        {
            using (var petitionRepo = new PetitionRepository())
            {
                return await petitionRepo.SyncPetitionsDataAsync(Configuration.LastSyncDate);
            }
        }

        async Task RetrivePetitionsFromParse()
        {
            var locationObjectId = await new BusinessRepository().GetLocationObjectIdAsync();
            var petitions = ParseObject.GetQuery("Petition")
                .WhereGreaterThanOrEqualTo("date", Configuration.LastSyncDate)
                .WhereEqualTo("location", locationObjectId);

            using (var petitionRepo = new PetitionRepository())
            {
                foreach (var petition in await petitions.FindAsync())
                {
                    var newPetition = new Petition()
                    {
                        ObjectId = petition.Get<string>("postgresId"),
                        Employee = petition.Get<string>("employee"),
                        Date = DateFunctions.FromDateTimeToLocalDate(petition.Get<DateTime>("date")),
                        AvailableFrom = DateFunctions.FromDateTimeToLocalTime(petition.Get<DateTime>("availableFrom")),
                        AvailableTo = DateFunctions.FromDateTimeToLocalTime(petition.Get<DateTime>("availableTo"))
                    };

                    petitionRepo.Petition = newPetition;
                    await petitionRepo.SaveAsync();
                }
            }
        }

        async Task<ParseObject> GetPetitionObject(string postgresId)
        {
            var _object = new ParseObject("Petition");

            try
            {
                _object = await ParseObject.GetQuery("Petition").WhereEqualTo("postgresId", postgresId).FirstOrDefaultAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("No se pudo establecer conexión a Internet.");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _object;
        }
    }
}
