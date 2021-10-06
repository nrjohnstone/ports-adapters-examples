using System;
using System.Linq;
using Adapter.Persistence.MySql.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Adapter.Persistence.MySql.Tests.Integration
{
    public class BookOrderRepositoryTests
    {
        private BookOrderRepository CreateSut()
        {
            string connectionString = "server=127.0.0.1;" +
                                      "uid=bookorder_srv;" +
                                      "pwd=123;" +
                                      "database=bookorders";

            return new BookOrderRepository(connectionString);
        }

        [Fact]
        public void ShouldBeAbleToStoreAndGetBookOrder()
        {
            var sut = CreateSut();

            Guid orderId = Guid.NewGuid();

            var newBookOrder = BookOrder.CreateNew("Foo", orderId);
            newBookOrder.AddBookRequest(new BookTitleRequest("Book1","Foo", 10.5M, 2));
            sut.Store(newBookOrder);

            var storedOrder = sut.Get(orderId);

            storedOrder.ShouldBeEquivalentTo(newBookOrder);
        }

        [Fact]
        public void ShouldBeAbleToRetrieveAllBookOrdersForASupplier()
        {
            var sut = CreateSut();

            var supplierFoo = $"Foo{Guid.NewGuid()}";
            var supplierBar = $"Bar{Guid.NewGuid()}";

            var bookOrder1 = BookOrder.CreateNew(supplierFoo, Guid.NewGuid());
            bookOrder1.AddBookRequest(new BookTitleRequest("Book1", supplierFoo, 10.5M, 2));

            var bookOrder2 = BookOrder.CreateNew(supplierBar, Guid.NewGuid());
            bookOrder2.AddBookRequest(new BookTitleRequest("Book2", supplierBar, 2.5M, 3));

            var bookOrder3 = BookOrder.CreateNew(supplierFoo, Guid.NewGuid());
            bookOrder3.AddBookRequest(new BookTitleRequest("Book3", supplierFoo, 5.5M, 4));

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);
            sut.Store(bookOrder3);

            var bookOrdersForFoo = sut.GetBySupplier(supplierFoo).ToList();

            bookOrdersForFoo.Should().Contain(x => x.Id == bookOrder1.Id);
            bookOrdersForFoo.Should().Contain(x => x.Id == bookOrder3.Id);

            var storedBookOrder1 = bookOrdersForFoo.First(x => x.Id == bookOrder1.Id);
            storedBookOrder1.Id.Should().Be(bookOrder1.Id);
            storedBookOrder1.Supplier.Should().Be(bookOrder1.Supplier);
            storedBookOrder1.State.Should().Be(bookOrder1.State);
            storedBookOrder1.OrderLines.ShouldBeEquivalentTo(bookOrder1.OrderLines);

            var storedBookOrder3 = bookOrdersForFoo.First(x => x.Id == bookOrder3.Id);
            storedBookOrder3.Id.Should().Be(bookOrder3.Id);
            storedBookOrder3.Supplier.Should().Be(bookOrder3.Supplier);
            storedBookOrder3.State.Should().Be(bookOrder3.State);
            storedBookOrder3.OrderLines.ShouldBeEquivalentTo(bookOrder3.OrderLines);
        }

        [Fact]
        public void ShouldBeAbleToRetrieveAllBookOrdersForASupplierAndState()
        {
            var sut = CreateSut();

            var supplierFoo = $"Foo{Guid.NewGuid()}";
            var supplierBar = $"Bar{Guid.NewGuid()}";

            var bookOrder1 = BookOrder.CreateNew(supplierFoo, Guid.NewGuid());
            bookOrder1.AddBookRequest(new BookTitleRequest("Book1", supplierFoo, 10.5M, 2));

            var bookOrder2 = BookOrder.CreateNew(supplierBar, Guid.NewGuid());
            bookOrder2.AddBookRequest(new BookTitleRequest("Book2", supplierBar, 2.5M, 3));
            bookOrder2.Approve();

            var bookOrder3 = BookOrder.CreateNew(supplierFoo, Guid.NewGuid());
            bookOrder3.AddBookRequest(new BookTitleRequest("Book3", supplierFoo, 5.5M, 4));
            bookOrder3.Approve();
            bookOrder3.Send();

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);
            sut.Store(bookOrder3);

            var bookOrdersForFoo = sut.GetBySupplier(supplierFoo, BookOrderState.Sent).ToList();

            bookOrdersForFoo.Should().Contain(x => x.Id == bookOrder3.Id);

            var storedBookOrder3 = bookOrdersForFoo.First(x => x.Id == bookOrder3.Id);
            storedBookOrder3.Id.Should().Be(bookOrder3.Id);
            storedBookOrder3.Supplier.Should().Be(bookOrder3.Supplier);
            storedBookOrder3.State.Should().Be(bookOrder3.State);
            storedBookOrder3.OrderLines.ShouldBeEquivalentTo(bookOrder3.OrderLines);
        }

        [Fact]
        public void ShouldBeAbleToRetrieveAllBookOrdersForAState()
        {
            var sut = CreateSut();

            var supplierFoo = $"Foo{Guid.NewGuid()}";
            var supplierBar = $"Bar{Guid.NewGuid()}";

            var bookOrder1 = BookOrder.CreateNew(supplierFoo, Guid.NewGuid());
            bookOrder1.AddBookRequest(new BookTitleRequest("Book1", supplierFoo, 10.5M, 2));

            var bookOrder2 = BookOrder.CreateNew(supplierBar, Guid.NewGuid());
            bookOrder2.AddBookRequest(new BookTitleRequest("Book2", supplierBar, 2.5M, 3));
            bookOrder2.Approve();

            var bookOrder3 = BookOrder.CreateNew(supplierFoo, Guid.NewGuid());
            bookOrder3.AddBookRequest(new BookTitleRequest("Book3", supplierFoo, 5.5M, 4));
            bookOrder3.Approve();
            bookOrder3.Send();

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);
            sut.Store(bookOrder3);

            var bookOrdersForFoo = sut.GetByState(BookOrderState.Sent).ToList();

            bookOrdersForFoo.Should().Contain(x => x.Id == bookOrder3.Id);

            var storedBookOrder3 = bookOrdersForFoo.First(x => x.Id == bookOrder3.Id);
            storedBookOrder3.Id.Should().Be(bookOrder3.Id);
            storedBookOrder3.Supplier.Should().Be(bookOrder3.Supplier);
            storedBookOrder3.State.Should().Be(bookOrder3.State);
            storedBookOrder3.OrderLines.ShouldBeEquivalentTo(bookOrder3.OrderLines);
        }

        [Fact]
        public void WhenBookOrderStateIsChanged_ShouldBeAbleToUpdateStoredBookOrder()
        {
            var sut = CreateSut();

            Guid orderId = Guid.NewGuid();

            var bookOrder = BookOrder.CreateNew("Foo", orderId);
            bookOrder.AddBookRequest(new BookTitleRequest("Book1", "Foo", 10.5M, 2));
            sut.Store(bookOrder);

            bookOrder.Approve();

            sut.Store(bookOrder);

            var storedBookOrder = sut.Get(orderId);

            storedBookOrder.ShouldBeEquivalentTo(bookOrder);
        }

        [Fact]
        public void WhenBookOrderHasLineAdded_ShouldBeAbleToUpdateStoredBookOrder()
        {
            var sut = CreateSut();

            Guid orderId = Guid.NewGuid();

            var bookOrder = BookOrder.CreateNew("Foo", orderId);
            bookOrder.AddBookRequest(new BookTitleRequest("Book1", "Foo", 10.5M, 2));
            sut.Store(bookOrder);

            bookOrder.AddBookRequest(new BookTitleRequest("Book2", "Foo", 22.0M, 1));
            sut.Store(bookOrder);

            var storedBookOrder = sut.Get(orderId);

            storedBookOrder.OrderLines.Count.Should().Be(2);
        }

    }
}