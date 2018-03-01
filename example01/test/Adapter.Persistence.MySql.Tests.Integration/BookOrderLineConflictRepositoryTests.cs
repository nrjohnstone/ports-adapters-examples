using System;
using System.Linq;
using Adapter.Persistence.MySql.Repositories;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Adapter.Persistence.MySql.Tests.Integration
{
    public class BookOrderLineConflictRepositoryTests
    {
        private BookOrderLineConflictRepository CreateSut()
        {
            string connectionString = "server=127.0.0.1;" +
                                      "uid=bookorder_srv;" +
                                      "pwd=123;" +
                                      "database=bookorders";

            return new BookOrderLineConflictRepository(connectionString);
        }

        [Fact]
        public void Get_ShouldReturnAllResults()
        {
            var sut = CreateSut();

            var results = sut.Get();

            results.Should().NotBeEmpty();
        }

        [Fact]
        public void ShouldBeAbleToStoreAndRetrieveUsingGetAll()
        {
            var sut = CreateSut();

            var bookOrderLineConflict = BookOrderLineQuantityConflict.CreateNew(Guid.NewGuid(), Guid.NewGuid(), 1);

            sut.Store(bookOrderLineConflict);

            var result = sut.Get();

            var storedConflict = result.Single(x => x.Id == bookOrderLineConflict.Id);
            storedConflict.BookOrderId.Should().Be(bookOrderLineConflict.BookOrderId);
            storedConflict.ConflictType.Should().Be(bookOrderLineConflict.ConflictType);
            storedConflict.BookOrderLineId.Should().Be(bookOrderLineConflict.BookOrderLineId);
            storedConflict.ConflictValue.Should().Be(bookOrderLineConflict.ConflictValue);
        }

        [Theory]
        [InlineData("Price")]
        [InlineData("Quantity")]
        public void ShouldBeAbleToStoreAndRetrieveUsingGetById_WhenConflictTypeIs(string conflictType)
        {
            var sut = CreateSut();

            BookOrderLineConflict bookOrderLineConflict = CreateConflict(conflictType);

            sut.Store(bookOrderLineConflict);

            var storedConflict = sut.Get(bookOrderLineConflict.Id);

            storedConflict.BookOrderId.Should().Be(bookOrderLineConflict.BookOrderId);
            storedConflict.ConflictType.Should().Be(bookOrderLineConflict.ConflictType);
            storedConflict.BookOrderLineId.Should().Be(bookOrderLineConflict.BookOrderLineId);
            storedConflict.ConflictValue.Should().Be(bookOrderLineConflict.ConflictValue);
        }

        private static BookOrderLineConflict CreateConflict(string conflictType)
        {
            if (conflictType.Equals("Quantity"))
                return BookOrderLineQuantityConflict.CreateNew(Guid.NewGuid(), Guid.NewGuid(), 1);

            if (conflictType.Equals("Price"))
                return BookOrderLinePriceConflict.CreateNew(Guid.NewGuid(), Guid.NewGuid(), 10.5M);

            throw new ArgumentOutOfRangeException();
        }

        [Fact]
        public void WhenNoConflictExists_ShouldReturnNull()
        {
            var sut = CreateSut();

            var storedConflict = sut.Get(Guid.NewGuid());

            storedConflict.Should().BeNull();
        }
    }
}