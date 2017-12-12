namespace Adapter.Persistence.CouchDb
{
    public class CouchDbSettings
    {
        public CouchDbSettings(string databaseUri, string databaseName)
        {
            DatabaseUri = databaseUri;
            DatabaseName = databaseName;
        }

        public string DatabaseUri { get; }
        public string DatabaseName { get; }
    }
}