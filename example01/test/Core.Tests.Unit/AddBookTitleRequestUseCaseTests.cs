using System;
using System.Linq;
using Adapter.Persistence.Test;
using Core.Tests.Unit.Helpers;
using Domain.Entities;
using Domain.Ports.Persistence;
using Domain.UseCases;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Core.Tests.Unit
{
    public class AddBookTitleRequestUseCaseTests
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public AddBookTitleRequestUseCaseTests()
        {
            _bookOrderRepository = new BookOrderRepository();
        }

        [Fact]
        public void OrderingABook_WhenOrderForSupplierDoesNotExist_ShouldCreateNewOrderForSupplier()
        {
            var sut = CreateSut();

            BookTitleRequest bookTitleRequest =
                a.BookTitleOrder
                .ForSupplier("SupplierFoo");

            // act
            sut.Execute(bookTitleRequest);

            // assert
            var storedOrders = _bookOrderRepository.GetBySupplier(bookTitleRequest.Supplier).ToList();

            storedOrders.Count.Should().Be(1);

            var storedOrder = storedOrders.First();
            storedOrder.Supplier.Should().Be("SupplierFoo");
            storedOrder.State.Should().Be(BookOrderState.New);
            storedOrder.OrderLines.Should().Contain(
                x => x.Title == bookTitleRequest.Title &&
                     x.Price == bookTitleRequest.Price &&
                     x.Quantity == bookTitleRequest.Quantity);
        }

        [Fact]
        public void OrderABook_WhenOrderForSupplierAlreadyExists_ShouldAddBooksToExistingOrder()
        {
            var sut = CreateSut();

            StoreBookOrderWithOrderLineForSupplier("SupplierBar");

            BookTitleRequest bookTitleRequest =
                a.BookTitleOrder
                    .ForSupplier("SupplierBar")
                    .ForTitle("The Color of Magic");

            // act
            sut.Execute(bookTitleRequest);

            // assert
            var storedOrders = _bookOrderRepository.GetBySupplier(bookTitleRequest.Supplier).ToList();

            storedOrders.Count.Should().Be(1);

            var storedOrder = storedOrders.First();

            storedOrder.Supplier.Should().Be("SupplierBar");
            storedOrder.OrderLines.Count.Should().Be(2);
            storedOrder.OrderLines.Should().Contain(
                x => x.Title == bookTitleRequest.Title &&
                     x.Price == bookTitleRequest.Price &&
                     x.Quantity == bookTitleRequest.Quantity);

        }

        [Fact]
        public void OrderABook_WhenOrderForDifferentSupplierExists_ShouldCreateNewOrder()
        {
            var sut = CreateSut();

            StoreBookOrderWithOrderLineForSupplier("SupplierBar");

            BookTitleRequest bookTitleRequest =
                a.BookTitleOrder
                    .ForSupplier("SupplierFoo")
                    .ForTitle("The Color of Magic");

            // act
            sut.Execute(bookTitleRequest);

            // assert
            var ordersForSupplierBar = _bookOrderRepository.GetBySupplier("SupplierBar").ToList();
            var ordersForSupplierFoo = _bookOrderRepository.GetBySupplier("SupplierFoo").ToList();

            ordersForSupplierBar.Count.Should().Be(1);
            ordersForSupplierFoo.Count.Should().Be(1);

            var firstOrderForSupplierFoo = ordersForSupplierFoo.First();

            firstOrderForSupplierFoo.Supplier.Should().Be("SupplierFoo");
            firstOrderForSupplierFoo.OrderLines.Count.Should().Be(1);
            firstOrderForSupplierFoo.OrderLines.Should().Contain(
                x => x.Title == bookTitleRequest.Title &&
                     x.Price == bookTitleRequest.Price &&
                     x.Quantity == bookTitleRequest.Quantity);
        }

        private BookOrder StoreBookOrderWithOrderLineForSupplier(string supplier)
        {
            BookOrder bookOrder = BookOrder.CreateNew(
                "SupplierBar",
                Guid.NewGuid());

            bookOrder.AddBookRequest(
                a.BookTitleOrder
                .ForSupplier(supplier)
                .ForTitle("The Hobbit")
                );
            _bookOrderRepository.Store(bookOrder);

            return bookOrder;
        }

        private AddBookTitleRequestUseCase CreateSut()
        {
            return new AddBookTitleRequestUseCase(
                _bookOrderRepository);
        }

        [Fact]
        public void OrderABook_WhenOrderForSupplierIsNotInNewState_ShouldCreateNewOrderForSupplier()
        {
            var sut = CreateSut();

            var bookOrder = StoreBookOrderWithOrderLineForSupplier("SupplierBar");
            bookOrder.Approve();
            _bookOrderRepository.Store(bookOrder);

            BookTitleRequest bookTitleRequest =
                a.BookTitleOrder
                    .ForSupplier("SupplierBar")
                    .ForTitle("The Color of Magic");

            // act
            sut.Execute(bookTitleRequest);

            // assert
            var storedOrders = _bookOrderRepository.GetBySupplier(bookTitleRequest.Supplier).ToList();

            storedOrders.Count.Should().Be(2);
            storedOrders.Select(x => x.Supplier).Should().OnlyContain(supplier => supplier.Equals("SupplierBar"));

            var newOrder = storedOrders.FirstOrDefault(x => x.Id != bookOrder.Id);

            newOrder.Should().NotBeNull("A new bookOrder should have been created");
            newOrder.Supplier.Should().Be("SupplierBar");
            newOrder.OrderLines.Count.Should().Be(1);
            newOrder.OrderLines.Should().Contain(
                x => x.Title == bookTitleRequest.Title &&
                     x.Price == bookTitleRequest.Price &&
                     x.Quantity == bookTitleRequest.Quantity);
        }
    }
}