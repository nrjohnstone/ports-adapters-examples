using System;
using System.Net;
using Adapter.Persistence.CouchDb.Views;
using FluentAssertions;
using MyCouch;

namespace Adapter.Persistence.CouchDb.Tests.Integration
{
    public abstract class CouchDbTestBase
    {
        protected readonly string DatabaseName;

        public CouchDbTestBase()
        {
            var randomDatabaseSuffix = Guid.NewGuid().ToString();
            DatabaseName = $"it-{randomDatabaseSuffix}";
            CreateDatabaseIfNotExists();
            CrewViews();
        }


        private void CrewViews()
        {
            new ViewManager(Constants.DatabaseUri, DatabaseName).CreateViews();
        }

        private void CreateDatabaseIfNotExists()
        {
            using (var couchDb = new MyCouchServerClient("http://admin:123@localhost:5984"))
            {
                if (couchDb.Databases.GetAsync(DatabaseName).Result.StatusCode == HttpStatusCode.OK)
                    return;

                var response = couchDb.Databases.PutAsync(DatabaseName);

                response.Result.StatusCode.Should().Be(HttpStatusCode.Created);
            }
        }

    }
}