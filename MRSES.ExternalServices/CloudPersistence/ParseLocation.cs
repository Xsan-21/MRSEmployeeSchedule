using System;
using System.Net.Http;
using System.Threading.Tasks;
using Parse;
using MRSES.Core.Entities;
using MRSES.Core.Persistence;

namespace MRSES.ExternalServices.CloudPersistence
{
    internal class ParseLocation
    {
        public async Task SyncAsync()
        {
            await UpdateOrSaveLocationAsync();
        }

        async Task<Location> GetLocationAsync()
        {
            using (var _businessRepo = new BusinessRepository())
            {
                // Since every location sync their own data, this will return only one location. 
                // The current program location.
                var location = await _businessRepo.SyncLocationAsync(Configuration.LastSyncDate);
                return location;
            }
        }

        async Task<bool> UpdateLocationAsync(Location location)
        {
            try
            {
                var existingLocation = await GetLocationObject(location.ObjectId);

                if (existingLocation == null)
                    return false;

                existingLocation["accessKey"] = location.AccessKey;
                existingLocation["city"] = location.City;
                existingLocation["phoneNumber"] = location.PhoneNumber;

                await existingLocation.SaveAsync();
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

        async Task SaveLocationAsync(Location location)
        {
            try
            { 
                var newLocation = new ParseObject("Location")
                {
                    { "postgresId", location.ObjectId },
                    { "business", location.Business },
                    { "accessKey", location.AccessKey},
                    { "city", location.City },
                    { "phoneNumber", location.PhoneNumber }
                };

                await newLocation.SaveAsync();
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

        async Task UpdateOrSaveLocationAsync()
        {
            var location = await GetLocationAsync();

            if (!string.IsNullOrEmpty(location.ObjectId))
            {
                var result = await UpdateLocationAsync(location);

                if (!result)
                    await SaveLocationAsync(location);
            }
        }

        static internal async Task<ParseObject> GetLocationObject(string postgresId)
        {
            var _object = new ParseObject("Location");

            try
            {
                _object = await ParseObject.GetQuery("Location").WhereEqualTo("postgresId", postgresId).FirstOrDefaultAsync();
            }
            catch (HttpRequestException)
            {
                throw new Exception("No se pudo establecer conexión a Internet.");
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return _object;
        }
    }
}
