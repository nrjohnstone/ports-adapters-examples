using System;
using Adapter.Persistence.CouchDb.Repositories;
using Domain.Entities;
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

        [Fact]
        public void Get_WhenOrderDoesNotExist_ShouldReturnNull()
        {
            var sut = CreateSut();

            var bookOrder = sut.Get(Guid.Parse("D5153312-1BFF-4529-99DA-A189BB050F48"));

            bookOrder.Should().BeNull();
        }

        [Fact]
        public void CanStoreAndRetrieve_Single_BookOrderLineConflict()
        {
            var sut = CreateSut();

            BookOrderLineConflict bookOrderLineConflict =
                BookOrderLinePriceConflict.CreateExisting(
                    Guid.NewGuid(),
                    Guid.NewGuid(),Guid.NewGuid(), 10.50M, false, DateTime.Now);

            sut.Store(bookOrderLineConflict);

            var retrievedEntity = sut.Get(bookOrderLineConflict.Id);

            retrievedEntity.Should().NotBeNull();
            retrievedEntity.Id.Should().Be(bookOrderLineConflict.Id);
            retrievedEntity.BookOrderId.Should().Be(bookOrderLineConflict.BookOrderId);
            retrievedEntity.BookOrderLineId.Should().Be(bookOrderLineConflict.BookOrderLineId);
            retrievedEntity.Accepted.Should().Be(bookOrderLineConflict.Accepted);
        }

        [Fact]
        public void CanStore_Multiple_BookOrderLineConflicts()
        {
            var sut = CreateSut();

            var bookOrderId = Guid.NewGuid();

            BookOrderLineConflict bookOrderLineConflict1 =
                BookOrderLinePriceConflict.CreateExisting(
                    Guid.NewGuid(),
                    bookOrderId, Guid.NewGuid(), 10.50M, false, DateTime.Now);

            BookOrderLineConflict bookOrderLineConflict2 =
                BookOrderLinePriceConflict.CreateExisting(
                    Guid.NewGuid(),
                    bookOrderId, Guid.NewGuid(), 10.50M, false, DateTime.Now);

            sut.Store(new []{ bookOrderLineConflict1, bookOrderLineConflict2});

            var retrievedConflict1 = sut.Get(bookOrderLineConflict1.Id);
            var retrievedConflict2 = sut.Get(bookOrderLineConflict2.Id);

            retrievedConflict1.Should().NotBeNull();
            retrievedConflict2.Should().NotBeNull();
        }
    }
}