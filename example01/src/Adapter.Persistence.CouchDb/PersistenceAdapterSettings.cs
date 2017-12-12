namespace Adapter.Persistence.CouchDb
{
    public class PersistenceAdapterSettings
    {
        public PersistenceAdapterSettings(string databaseUri, string databaseName)
        {
            DatabaseUri = databaseUri;
            DatabaseName = databaseName;
        }

        public string DatabaseUri { get;  }
        public string DatabaseName { get; }
    }
}