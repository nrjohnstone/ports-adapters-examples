using System;
using Adapter.Persistence.MySql.Repositories;
using Domain.Ports.Persistence;
using SimpleInjector;

namespace Adapter.Persistence.MySql
{
    public class PersistenceAdapter
    {
        private readonly PersistenceAdapterSettings _settings;
        private bool _initialized;

        public PersistenceAdapter(PersistenceAdapterSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
        }

        public void Initialize()
        {
            _initialized = true;
        }

        public void Register(Container container)
        {
            if (!_initialized)
                throw new AdpaterNotInitializedException();

            BookOrderRepository bookOrderRepository = new BookOrderRepository(_settings.ConnectionString);
            var bookOrderLineConflictRepository = new BookOrderLineConflictRepository(_settings.ConnectionString);

            container.RegisterSingleton<IBookOrderRepository>(bookOrderRepository);
            container.RegisterSingleton<IBookOrderLineConflictRepository>(bookOrderLineConflictRepository);
        }
    }
}