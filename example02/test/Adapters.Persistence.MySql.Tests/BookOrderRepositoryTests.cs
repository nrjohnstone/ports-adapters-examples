using System;
using System.Linq;
using Adapters.Persistence.MySql.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
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

        [Fact]
        public void CanStoreAndRetrieve_ANewBookOrder_WithEditedLinePrice()
        {
            var sut = CreateSut();
            var orderId = Guid.NewGuid();
            var order = BookOrder.CreateNew("SomeSupplier", orderId);
            order.AddBookRequest(new BookTitleOrder("Title1", "SomeSupplier", 10.5M, 5));

            var orderLine = order.OrderLines.First(x => x.Title.Equals("Title1"));
            order.UpdateOrderLinePrice(orderLine.Id, 20.5M);

            sut.Store(order);

            var bookOrder = sut.Get(orderId);

            bookOrder.OrderLines.First(x => x.Id == orderLine.Id).Price.Should().Be(20.5M);
        }

        [Fact]
        public void CanStoreAndRetrieve_ANewBookOrder_WithRemovedLine()
        {
            var sut = CreateSut();
            var orderId = Guid.NewGuid();
            var order = BookOrder.CreateNew("SomeSupplier", orderId);
            order.AddBookRequest(new BookTitleOrder("Title1", "SomeSupplier", 10.5M, 5));
            order.AddBookRequest(new BookTitleOrder("Title2", "SomeSupplier", 20.5M, 7));

            var orderLineToRemove = order.OrderLines.First(x => x.Title.Equals("Title2"));
            order.RemoveOrderLine(orderLineToRemove.Id);

            sut.Store(order);

            var bookOrder = sut.Get(orderId);

            bookOrder.Supplier.Should().Be("SomeSupplier");
            bookOrder.Id.Should().Be(orderId);
            bookOrder.State.Should().Be(BookOrderState.New);
            bookOrder.OrderLines.Count.Should().Be(1);
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title.Equals("Title1") && x.Price == 10.5M && x.Quantity == 5);
        }
    }
}