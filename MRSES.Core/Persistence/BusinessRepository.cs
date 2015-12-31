using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MRSES.Core.Entities;
using Npgsql;
using NpgsqlTypes;
using MRSES.Core.Shared;

namespace MRSES.Core.Persistence
{
    public interface IBusinessRepository : IDatabase
    {
        Task<List<Business>> GetBusinessesAsync();
        Task<List<Location>> GetBusinessLocationsAsync(string business);
        Task<string> GetLocationObjectIdAsync();
        Task<string> GetLocationObjectIdAsync(string city);
        Task<string> GetBusinessObjectIdAsync();
        Task<string[]> GetDepartmentsAsync();
        IBusiness Business { get; set; }
        ILocation Location { get; set; }
    }

    public class BusinessRepository : IBusinessRepository
    {
        public IBusiness Business { get; set; }
        public ILocation Location { get; set; }

        public BusinessRepository() { }
        public BusinessRepository(IBusiness business, ILocation location)
        {
            Business = business;
            Location = location;
        }

        public async Task SaveAsync()
        {
            if (Business != null)
            {
                ValidateRequiredDataForAction("business");
                await SaveBusinessAsync(); 
            }

            if (Location != null)
            {
                ValidateRequiredDataForAction("location");
                await SaveLocationAsync(); 
            }
        }

        async Task SaveBusinessAsync()
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("SaveBusiness");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Business.ObjectId);
                    command.Parameters.AddWithValue("business_name", NpgsqlDbType.Text, Business.Name);
                    command.Parameters.AddWithValue("first_week_day", NpgsqlDbType.Text, Business.FirstDayOfWeek);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        async Task SaveLocationAsync()
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("SaveLocation");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("object_id", NpgsqlDbType.Text, Location.ObjectId);
                    command.Parameters.AddWithValue("business_id", NpgsqlDbType.Text, Business.ObjectId);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Location.AccessKey);
                    command.Parameters.AddWithValue("city", NpgsqlDbType.Text, Location.City);
                    command.Parameters.AddWithValue("phone", NpgsqlDbType.Text, Location.PhoneNumber);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<string> GetBusinessObjectIdAsync()
        {
            var objectId = "";

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetBusinessObjectID");
                    command.CommandType = System.Data.CommandType.Text;

                    command.Parameters.AddWithValue("business", NpgsqlDbType.Text, Configuration.Business);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            objectId = await reader.GetFieldValueAsync<string>(0);
                    }
                }
            }

            return objectId;
        }

        public async Task<string> GetLocationObjectIdAsync()
        {
            return await GetLocationObjectIdAsync(Configuration.Location);
        }

        public async Task<string> GetLocationObjectIdAsync(string location)
        {
            var objectId = "";

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetLocationObjectID");
                    command.CommandType = System.Data.CommandType.Text;

                    command.Parameters.AddWithValue("business", NpgsqlDbType.Text, Configuration.Business);
                    command.Parameters.AddWithValue("location", NpgsqlDbType.Text, location);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            objectId = await reader.GetFieldValueAsync<string>(0);
                    }
                }
            }

            return objectId;
        }

        public async Task<List<Business>> GetBusinessesAsync()
        {
            var businesses = new List<Business>();
            var business = new Business();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetBusinesses");
                    command.CommandType = System.Data.CommandType.Text;

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            business.ObjectId = await reader.GetFieldValueAsync<string>(0);
                            business.Name = await reader.GetFieldValueAsync<string>(1);
                            business.FirstDayOfWeek = await reader.GetFieldValueAsync<string>(2);
                            businesses.Add(business);
                        }
                    }
                }
            }

            return businesses;
        }

        public async Task<Business> SyncBusinessAsync(DateTime lastSyncDate)
        {
            var business = new Business();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "select * from sync_businesses(:sync_date, :business)";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("sync_date", NpgsqlDbType.Timestamp, lastSyncDate);
                    command.Parameters.AddWithValue("business", NpgsqlDbType.Text, Configuration.Business);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            business.ObjectId = await reader.GetFieldValueAsync<string>(0);
                            business.Name = await reader.GetFieldValueAsync<string>(1);
                            business.FirstDayOfWeek = await reader.GetFieldValueAsync<string>(2);
                        }
                    }
                }
            }

            return business;
        }

        public async Task<Location> SyncLocationAsync(DateTime lastSyncDate)
        {
            var location = new Location();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "select * from sync_locations(:sync_date, :access_key)";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("sync_date", NpgsqlDbType.Timestamp, lastSyncDate);
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            location.ObjectId = await reader.GetFieldValueAsync<string>(0);
                            location.Business = await reader.GetFieldValueAsync<string>(1);
                            location.AccessKey = await reader.GetFieldValueAsync<string>(2);
                            location.City = await reader.GetFieldValueAsync<string>(3);
                            location.PhoneNumber = await reader.GetFieldValueAsync<string>(4);
                        }
                    }
                }
            }

            return location;
        }

        public async Task<List<Location>> GetBusinessLocationsAsync(string business)
        {
            var locations = new List<Location>();
            var location = new Location();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetBusinessLocations");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("business_name", NpgsqlDbType.Varchar, business);   

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            location.ObjectId = await reader.GetFieldValueAsync<string>(0);
                            location.Business = await reader.GetFieldValueAsync<string>(1);
                            location.AccessKey = await reader.GetFieldValueAsync<string>(2);
                            location.City = await reader.GetFieldValueAsync<string>(3);
                            location.PhoneNumber = await reader.GetFieldValueAsync<string>(4);
                            locations.Add(location);
                        }
                    }
                }
            }

            return locations;
        }

        public async Task<string[]> GetDepartmentsAsync()
        {
            var departments = new List<string>();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = GetQuery("GetDepartments");
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("access_key", NpgsqlDbType.Text, Configuration.AccessKey);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string result = await reader.GetFieldValueAsync<string>(0);
                            departments.Add(result);
                        }                        
                    }
                }
            }

            return departments.ToArray();
        }

        string GetQuery(string action)
        {
            string query = string.Empty;
            switch (action)
            {
                case "SaveBusiness":
                    query = @"SELECT add_business(:object_id, :business_name, :first_week_day)";
                    break;
                case "GetBusinesses":
                    query = @"SELECT * FROM get_businesses()";
                    break;
                case "SaveLocation":
                    query = @"SELECT add_location(:object_id, :business_id, :access_key, :city, :phone)";
                    break;
                case "GetDepartments":
                    query = @"SELECT get_departments(:access_key)";
                    break;
                case "GetBusinessLocations":
                    query = @"SELECT * FROM get_business_locations(:business_name)";
                    break;
                case "GetLocationObjectID":
                    query = "SELECT get_location_object_id(:business, :location)";
                    break;
                case "GetBusinessObjectID":
                    query = "SELECT get_business_object_id(:business)";
                    break;
                default:
                    break;
            }

            return query;
        }

        public void ValidateRequiredDataForAction(string dataToValidate)
        {
            if (dataToValidate == "business")
                ValidateBusiness();
            else if(dataToValidate == "location")
                ValidateLocation();
        }

        void ValidateBusiness()
        {
            if (StringFunctions.StringIsNullOrEmpty(Business.Name) || StringFunctions.StringIsNullOrEmpty(Business.FirstDayOfWeek))
                throw new ArgumentException("No ha indicado el nombre de su empresa o primer dia de jornada en la semana", "Business name and first day of work week required");

            if (StringFunctions.StringIsNullOrEmpty(Business.ObjectId))
                Business.ObjectId = StringFunctions.GenerateObjectId(10);
        }

        void ValidateLocation()
        {
            if ((Business == null || 
                StringFunctions.StringIsNullOrEmpty(Business.ObjectId)) || 
                StringFunctions.StringIsNullOrEmpty(Location.AccessKey) ||
                StringFunctions.StringIsNullOrEmpty(Location.City) ||
                StringFunctions.StringIsNullOrEmpty(Location.PhoneNumber))
                throw new ArgumentException("No se ha completado la información requerida de la localidad seleccionada. ", "Business and location information is required");

            if (StringFunctions.StringIsNullOrEmpty(Location.ObjectId))
                Location.ObjectId = StringFunctions.GenerateObjectId(10);           
        }

        public void Dispose()
        {
            Business = null;
            Location = null;
        }
    }
}
