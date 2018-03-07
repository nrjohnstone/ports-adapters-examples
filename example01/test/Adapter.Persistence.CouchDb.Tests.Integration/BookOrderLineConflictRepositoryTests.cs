using System;
using System.Net;
using Adapter.Persistence.CouchDb.Repositories;
using Adapter.Persistence.CouchDb.Views;
using FluentAssertions;
using MyCouch;
using Xunit;

namespace Adapter.Persistence.CouchDb.Tests.Integration
{
    public class BookOrderLineConflictRepositoryTests
    {
        private readonly string _databaseName;

        public BookOrderLineConflictRepositoryTests()
        {
            var randomDatabaseSuffix = Guid.NewGuid().ToString();
            _databaseName = $"it-{randomDatabaseSuffix}";
            CreateDatabaseIfNotExists();
            CrewViews();
        }

        private void CrewViews()
        {
            new ViewManager(Constants.DatabaseUri, _databaseName).CreateViews();
        }

        private void CreateDatabaseIfNotExists()
        {
            using (var couchDb = new MyCouchServerClient("http://admin:123@localhost:5984"))
            {
                if (couchDb.Databases.GetAsync(_databaseName).Result.StatusCode == HttpStatusCode.OK)
                    return;

                var response = couchDb.Databases.PutAsync(_databaseName);

                response.Result.StatusCode.Should().Be(HttpStatusCode.Created);
            }
        }

        private BookOrderLineConflictRepository CreateSut()
        {
            CouchDbSettings settings = new CouchDbSettings(
                Constants.DatabaseUri, _databaseName);
            return new BookOrderLineConflictRepository(settings);
        }

        [Fact]
        public void ShouldCreateInstance()
        {
            var sut = CreateSut();

            sut.Should().NotBeNull();
        }
    }
}