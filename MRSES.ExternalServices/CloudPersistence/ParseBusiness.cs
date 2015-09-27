using System;
using System.Threading.Tasks;
using Parse;
using MRSES.Core.Entities;
using MRSES.Core.Persistence;
using System.Net.Http;

namespace MRSES.ExternalServices.CloudPersistence
{
    internal class ParseBusiness
    {
        BusinessRepository _businessRepo;
          
        internal async Task SyncAsync()
        {
            using (_businessRepo = new BusinessRepository())
            {
                var business = await _businessRepo.SyncBusinessAsync(Configuration.LastSyncDate);
                var update = await UpdateBusinessAsync(business);

                if(!update)
                    await SaveBusinesAsync(business);
            }
        }

        async Task<bool> UpdateBusinessAsync(Business business)
        {
            try
            {
                var existingBusiness = await GetBusinessObject(business.ObjectId);

                if (existingBusiness == null)
                    return false;

                existingBusiness["firstDayOfWeek"] = business.FirstDayOfWeek;
                await existingBusiness.SaveAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("No se pudo establecer conexión a Internet.");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        async Task SaveBusinesAsync(Business business)
        {
            if (business.ObjectId == null)
                return;

            try
            {
                var newBusiness = new ParseObject("Business")
                {
                    { "postgresId", business.ObjectId },
                    { "name", business.Name},
                    { "firstDayOfWeek", business.FirstDayOfWeek},
                };

                await newBusiness.SaveAsync();
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

        static internal async Task<ParseObject> GetBusinessObject(string postgresId)
        {
            var _object = new ParseObject("Business");

            try
            {
                _object = await ParseObject.GetQuery("Business").WhereEqualTo("postgresId", postgresId).FirstOrDefaultAsync();
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
