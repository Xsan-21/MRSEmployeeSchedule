using Npgsql;
using NpgsqlTypes;
using System.Threading.Tasks;

namespace MRSES.Core.Persistence
{
    public struct ObjectId
    {
        static async public Task Change(string table_name, string oldId, string newId)
        {
            using (var dbConnection = new NpgsqlConnection(Configuration.DbConnection))
            {
                using (var command = new NpgsqlCommand("", dbConnection))
                {
                    command.CommandText = "SELECT change_object_id(:table_name, :old_object_id, :new_object_id)";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("table_name", NpgsqlDbType.Text, table_name);
                    command.Parameters.AddWithValue("old_object_id", NpgsqlDbType.Text, oldId);
                    command.Parameters.AddWithValue("new_object_id", NpgsqlDbType.Text, newId);

                    await command.Connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
