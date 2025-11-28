using Microsoft.Data.Sqlite;
using Dapper;

namespace CodingTracker.Data
{
    internal static class DatabaseInitializer
    {
        public static void Initialize()
        {
            using var connection = Database.CreateConnection();
            connection.Open();

            var sql = @"
                CREATE TABLE IF NOT EXISTS CodingSessions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL,
                    DurationMinutes INTEGER NOT NULL
                );
            ";
            connection.Execute(sql);
        }
    }
}
