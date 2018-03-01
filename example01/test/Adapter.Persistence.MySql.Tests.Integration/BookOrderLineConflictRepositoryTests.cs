using Adapter.Persistence.MySql.Repositories;
using FluentAssertions;
using Xunit;

namespace Adapter.Persistence.MySql.Tests.Integration
{
    public class BookOrderLineConflictRepositoryTests
    {
        private BookOrderLineConflictRepository CreateSut()
        {
            string connectionString = "server=127.0.0.1;" +
                                      "uid=bookorder_service;" +
                                      "pwd=123;" +
                                      "database=bookorders";

            return new BookOrderLineConflictRepository(connectionString);
        }

        [Fact]
        public void Get_ShouldReturnAllResults()
        {
            var sut = CreateSut();

            var results = sut.Get();

            results.Should().BeEmpty();
        }
    }
}