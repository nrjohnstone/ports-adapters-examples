using System;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Adapters.Persistence.EventStore.Tests
{
    public class BookOrderRepositoryTests
    {
        private BookOrderRepository CreateSut()
        {
            return new BookOrderRepository();
        }

        [Fact]
        public void Should()
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