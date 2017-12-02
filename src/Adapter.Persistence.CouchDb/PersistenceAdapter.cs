using Adapter.Persistence.CouchDb.Repositories;
using Domain.Ports.Persistence;
using SimpleInjector;

namespace Adapter.Persistence.CouchDb
{
    public class PersistenceAdapter
    {
        private readonly PersistenceAdapterSettings _settings;

        public PersistenceAdapter(PersistenceAdapterSettings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            
        }

        public void Register(Container container)
        {
            CouchDbSettings couchDbSettings = new CouchDbSettings(
                _settings.DatabaseUri, _settings.DatabaseName);
            container.RegisterSingleton(couchDbSettings);
            container.Register<IBookOrderRepository, BookOrderRepository>();
        }
    }
}