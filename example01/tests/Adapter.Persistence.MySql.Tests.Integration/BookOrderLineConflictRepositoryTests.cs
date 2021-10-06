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
        public void ShouldBeAbleToStoreMultipleConflictsAndRetrieveUsingGetAll()
        {
            var sut = CreateSut();

            var bookOrderLineConflict1 = BookOrderLineQuantityConflict.CreateExisting(Guid.NewGuid(),
                Guid.NewGuid(), Guid.NewGuid(), 1, true, DateTime.UtcNow);
            var bookOrderLineConflict2 = BookOrderLinePriceConflict.CreateExisting(Guid.NewGuid(),
                Guid.NewGuid(), Guid.NewGuid(), 30.25M, false, DateTime.UtcNow);

            sut.Store(new BookOrderLineConflict[] {bookOrderLineConflict1, bookOrderLineConflict2 });

            var result = sut.Get();

            var storedConflict1 = result.Single(x => x.Id == bookOrderLineConflict1.Id);
            storedConflict1.BookOrderId.Should().Be(bookOrderLineConflict1.BookOrderId);
            storedConflict1.ConflictType.Should().Be(bookOrderLineConflict1.ConflictType);
            storedConflict1.BookOrderLineId.Should().Be(bookOrderLineConflict1.BookOrderLineId);
            storedConflict1.ConflictValue.Should().Be(bookOrderLineConflict1.ConflictValue);
            storedConflict1.Accepted.Should().Be(bookOrderLineConflict1.Accepted);

            var storedConflict2 = result.Single(x => x.Id == bookOrderLineConflict2.Id);
            storedConflict2.BookOrderId.Should().Be(bookOrderLineConflict2.BookOrderId);
            storedConflict2.ConflictType.Should().Be(bookOrderLineConflict2.ConflictType);
            storedConflict2.BookOrderLineId.Should().Be(bookOrderLineConflict2.BookOrderLineId);
            storedConflict2.ConflictValue.Should().Be(bookOrderLineConflict2.ConflictValue);
            storedConflict2.Accepted.Should().Be(bookOrderLineConflict2.Accepted);
        }

        [Fact]
        public void ShouldBeAbleToStoreAndRetrieveUsingGetAll()
        {
            var sut = CreateSut();

            var bookOrderLineConflict = BookOrderLineQuantityConflict.CreateExisting(Guid.NewGuid(),
                Guid.NewGuid(), Guid.NewGuid(), 1, true, DateTime.UtcNow);

            sut.Store(bookOrderLineConflict);

            var result = sut.Get();

            var storedConflict = result.Single(x => x.Id == bookOrderLineConflict.Id);
            storedConflict.BookOrderId.Should().Be(bookOrderLineConflict.BookOrderId);
            storedConflict.ConflictType.Should().Be(bookOrderLineConflict.ConflictType);
            storedConflict.BookOrderLineId.Should().Be(bookOrderLineConflict.BookOrderLineId);
            storedConflict.ConflictValue.Should().Be(bookOrderLineConflict.ConflictValue);
            storedConflict.Accepted.Should().Be(bookOrderLineConflict.Accepted);
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
            storedConflict.ConflictType.ToString().Should().Be(conflictType);
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