using System;
using Adapters.Persistence.MySql.Repositories;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Adapters.Persistence.MySql.Tests
{
    public class BookOrderRepositoryTests
    {
        private BookOrderRepository CreateSut()
        {
            string connectionString = "server=127.0.0.1;" +
                                      "uid=bookorder_service;" +
                                      "pwd=123;" +
                                      "database=bookorders";

            return new BookOrderRepository(connectionString);
        }

        [Fact]
        public void CanStoreAndRetrieve_ANewBookOrder()
        {
            var sut = CreateSut();
            var orderId = Guid.NewGuid();
            sut.Store(BookOrder.CreateNew("SomeSupplier", orderId));

            var bookOrder = sut.Get(orderId);

            bookOrder.Supplier.Should().Be("SomeSupplier");
            bookOrder.Id.Should().Be(orderId);
            bookOrder.State.Should().Be(BookOrderState.New);
        }
    }
}