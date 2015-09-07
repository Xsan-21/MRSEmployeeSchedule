using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace MRSES.Core.Persistence
{
    public class DeleteDataRepository
    {
        public async Task<Dictionary<string,string>> GetDeletedObjectsAsync(DateTime sync_date)
        {
            var records = new Dictionary<string,string>();

            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "select * from get_deleted_records(:sync_date)";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("sync_date", NpgsqlDbType.Timestamp, sync_date);

                    await command.Connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string postgresId = await reader.GetFieldValueAsync<string>(0);
                            string className = await reader.GetFieldValueAsync<string>(1);
                            records.Add(postgresId, className);
                        }
                    }
                }
            }

            return records;
        }

        public async Task DeleteObjectsAsync(DateTime sync_date)
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "delete from deleted_records where deleted_at <= :sync_date";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("sync_date", NpgsqlDbType.Timestamp, sync_date);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
