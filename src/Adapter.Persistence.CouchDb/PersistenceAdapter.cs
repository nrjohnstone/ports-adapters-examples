using System;
using System.Net;
using Adapter.Persistence.CouchDb.Repositories;
using Adapter.Persistence.CouchDb.Views;
using Domain.Ports.Persistence;
using MyCouch;
using SimpleInjector;

namespace Adapter.Persistence.CouchDb
{
    public class PersistenceAdapter
    {
        private readonly PersistenceAdapterSettings _settings;
        private readonly CouchDbSettings _couchDbSettings;

        public PersistenceAdapter(PersistenceAdapterSettings settings)
        {
            _settings = settings;
            _couchDbSettings = new CouchDbSettings(
                _settings.DatabaseUri, _settings.DatabaseName);
        }

        public void Initialize()
        {
            CreateDatabaseIfNotExists();
            CrewViews();
        }

        public void Register(Container container)
        {
           
            container.RegisterSingleton(_couchDbSettings);
            container.Register<IBookOrderRepository, BookOrderRepository>();
        }

        private void CreateDatabaseIfNotExists()
        {
            using (var couchDb = new MyCouchServerClient(_couchDbSettings.DatabaseUri))
            {
                if (couchDb.Databases.GetAsync(_couchDbSettings.DatabaseName).Result.StatusCode == HttpStatusCode.OK)
                    return;

                var response = couchDb.Databases.PutAsync(_couchDbSettings.DatabaseName);

                if (response.Result.StatusCode != HttpStatusCode.Created)
                    throw new InvalidOperationException("Unable to create database");
            }
        }

        private void CrewViews()
        {
            new ViewManager(_settings.DatabaseUri, _couchDbSettings.DatabaseName).CreateViews();
        }

    }
}