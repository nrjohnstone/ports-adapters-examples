namespace Adapter.Persistence.CouchDb.Tests.Integration
{
    internal class Constants
    {
        public const string DatabaseName = "bookorders-db";
        public const string DatabaseUser = "admin";
        public const string DatabasePassword = "123";
        public const string DatabaseHost = "localhost";
        public const string DatabasePort = "5984";
        public static string DatabaseUri = $"http://{Constants.DatabaseUser}:{Constants.DatabasePassword}@{Constants.DatabaseHost}:{Constants.DatabasePort}";
    }
}