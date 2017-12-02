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
        }
    }
}