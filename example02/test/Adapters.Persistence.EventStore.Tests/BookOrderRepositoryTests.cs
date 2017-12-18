using System;
using Domain.Entities;
using Domain.ValueObjects;
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

        [Fact]
        public void CanStoreAndRetrieve_ANewBookOrder_WithLines()
        {
            var sut = CreateSut();
            var orderId = Guid.NewGuid();
            var order = BookOrder.CreateNew("SomeSupplier", orderId);
            order.AddBookRequest(new BookTitleOrder("Title1", "SomeSupplier", 10.5M, 5));
            order.AddBookRequest(new BookTitleOrder("Title2", "SomeSupplier", 20.5M, 7));

            sut.Store(order);

            var bookOrder = sut.Get(orderId);

            bookOrder.Supplier.Should().Be("SomeSupplier");
            bookOrder.Id.Should().Be(orderId);
            bookOrder.State.Should().Be(BookOrderState.New);
            bookOrder.OrderLines.Count.Should().Be(2);
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title.Equals("Title1") && x.Price == 10.5M && x.Quantity == 5);
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title.Equals("Title2") && x.Price == 20.5M && x.Quantity == 7);
        }
    }
}