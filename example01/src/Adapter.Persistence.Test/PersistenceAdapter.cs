using Domain.Ports.Persistence;
using SimpleInjector;

namespace Adapter.Persistence.Test
{
    public class PersistenceAdapter
    {
        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;
        }

        public void Register(Container container)
        {
            if (!_initialized)
                throw new AdpaterNotInitializedException();

            container.RegisterSingleton<IBookOrderRepository, BookOrderRepository>();
        }
    }
}