using Microsoft.Data.Sqlite;
using System.Data;
using System.Xml.Linq;

namespace CodingTracker.Data
{
    internal static class Database
    {
        private static readonly string _connectionString;

        static Database()
        {
            var config = XDocument.Load("Config.xml");
            var dbPath = config.Root.Element("DatabaseFilePath")?.Value;
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new Exception("DatabaseFilePath missing from Config.xml");
            _connectionString = $"Data Source={dbPath}";
        }
        public static IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}