using Adapter.Persistence.CouchDb.Repositories;
using FluentAssertions;
using Xunit;

namespace Adapter.Persistence.CouchDb.Tests.Integration
{
    public class BookOrderLineConflictRepositoryTests : CouchDbTestBase
    {
        private BookOrderLineConflictRepository CreateSut()
        {
            CouchDbSettings settings = new CouchDbSettings(
                Constants.DatabaseUri, DatabaseName);
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