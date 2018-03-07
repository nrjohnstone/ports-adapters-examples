using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Adapter.Persistence.CouchDb.Repositories;
using Adapter.Persistence.CouchDb.Views;
using Domain.Entities;
using FluentAssertions;
using MyCouch;
using Xunit;

namespace Adapter.Persistence.CouchDb.Tests.Integration
{
    public class BookOrderRepositoryTests
    {
        private readonly string _databaseName;

        public BookOrderRepositoryTests()
        {
            var randomDatabaseSuffix = Guid.NewGuid().ToString();
            _databaseName = $"it-{randomDatabaseSuffix}";
            CreateDatabaseIfNotExists();
            CrewViews();
        }

        private void CrewViews()
        {
            new ViewManager(Constants.DatabaseUri, _databaseName).CreateViews();
        }

        private void CreateDatabaseIfNotExists()
        {
            using (var couchDb = new MyCouchServerClient("http://admin:123@localhost:5984"))
            {
                if (couchDb.Databases.GetAsync(_databaseName).Result.StatusCode == HttpStatusCode.OK)
                    return;

                var response = couchDb.Databases.PutAsync(_databaseName);

                response.Result.StatusCode.Should().Be(HttpStatusCode.Created);
            }
        }

        private BookOrderRepository CreateSut()
        {
            CouchDbSettings settings = new CouchDbSettings(
                Constants.DatabaseUri, _databaseName);
            return new BookOrderRepository(settings);
        }

        [Fact]
        public void CanCreateInstance()
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
        public void Store_WhenBookOrderDoesNotExist_ShouldInsertBookOrder()
        {
            Guid bookOrderId = Guid.NewGuid();
            IEnumerable<OrderLine> orderLines = new List<OrderLine>()
            {
                new OrderLine("Line1Book", 100M, 2, Guid.NewGuid()) };

            var bookOrder = BookOrder.CreateExisting("AcmeBooks", bookOrderId, BookOrderState.New,
                orderLines);

            var sut = CreateSut();

            sut.Store(bookOrder);

            // assert
            var storedBookOrder = sut.Get(bookOrderId);
            storedBookOrder.Should().NotBeNull();
            storedBookOrder.Id.Should().Be(bookOrderId);
            storedBookOrder.OrderLines.ShouldBeEquivalentTo(bookOrder.OrderLines);
            storedBookOrder.State.Should().Be(bookOrder.State);
            storedBookOrder.Supplier.Should().Be(bookOrder.Supplier);
        }

        [Fact]
        public void Store_WhenBookOrderExists_ShouldUpdateBookOrder()
        {
            Guid bookOrderId = Guid.NewGuid();
            IEnumerable<OrderLine> orderLines = new List<OrderLine>()
            {
                new OrderLine("Line1Book", 100M, 2, Guid.NewGuid()) };

            var bookOrder = BookOrder.CreateExisting("AcmeBooks", bookOrderId, BookOrderState.New,
                orderLines);

            var sut = CreateSut();

            sut.Store(bookOrder);

            bookOrder.Approve();
            sut.Store(bookOrder);

            // assert
            var storedBookOrder = sut.Get(bookOrderId);
            storedBookOrder.Should().NotBeNull();
            storedBookOrder.Id.Should().Be(bookOrderId);
            storedBookOrder.OrderLines.ShouldBeEquivalentTo(bookOrder.OrderLines);
            storedBookOrder.State.Should().Be(BookOrderState.Approved);
            storedBookOrder.Supplier.Should().Be(bookOrder.Supplier);
        }

        [Theory]
        [InlineData("Foo")]
        [InlineData("Baz")]
        public void GetBySupplier_WhenBookOrdersForAllSuppliersExist_ShouldOnlyReturnBookOrdersForSupplier(string supplierToFilterBy)
        {
            var sut = CreateSut();

            Guid bookOrderId = Guid.NewGuid();
            IEnumerable<OrderLine> orderLines = new List<OrderLine>()
            {
                new OrderLine("Line1Book", 100M, 2, Guid.NewGuid()) };

            var bookOrder = BookOrder.CreateExisting(supplierToFilterBy, bookOrderId, BookOrderState.New,
                orderLines);
            sut.Store(bookOrder);

            var results = sut.GetBySupplier(supplierToFilterBy).ToList();

            results.Should().NotBeNull();
            results.Select(x => x.Supplier).Should().OnlyContain(y => y == supplierToFilterBy);
        }

        [Fact]
        public void GetBySupplier_ShouldIgnorePartialMatches()
        {
            var sut = CreateSut();

            var bookOrder1 = BookOrder.CreateExisting("Foo1", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 100M, 2, Guid.NewGuid())
                });
            var bookOrder2 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);

            var results = sut.GetBySupplier("Foo").ToList();

            results.Select(x => x.Supplier).Should().OnlyContain(y => y == "Foo");
        }

        [Fact]
        public void GetByState_ShouldOnlyReturnMatchesForState()
        {
            var sut = CreateSut();

            var bookOrder1 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.Sent,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 100M, 2, Guid.NewGuid())
                });
            var bookOrder2 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);

            var results = sut.GetByState(BookOrderState.Sent).ToList();

            results.Should().NotBeNull();
            results.Select(x => x.State).Should().OnlyContain(orderstate => orderstate == BookOrderState.Sent);
        }

        [Fact]
        public void GetBySupplierAndState_ShouldOnlyReturnExactMatches()
        {
            var sut = CreateSut();

            var bookOrder1 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.Sent,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 100M, 2, Guid.NewGuid())
                });
            var bookOrder2 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });
            var bookOrder3 = BookOrder.CreateExisting("Baz", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);
            sut.Store(bookOrder3);

            var results = sut.GetBySupplier("Foo", BookOrderState.New).ToList();

            results.Should().NotBeNull();
            results.Select(x => x.State).Should().OnlyContain(orderstate => orderstate == BookOrderState.New);
            results.Select(x => x.Supplier).Should().OnlyContain(supplier => supplier == "Foo");
        }

        [Fact]
        public void GetBySupplierAndState_WhenNoMatchesForSupplier_ShouldReturnEmptyList()
        {
            var sut = CreateSut();
            var results = sut.GetBySupplier("Foo", BookOrderState.New).ToList();

            results.Should().BeEmpty();
        }

        [Fact]
        public void GetBySupplierAndState_WhenNoMatchesForState_ShouldReturnEmptyList()
        {
            var sut = CreateSut();
            var bookOrder1 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.Sent,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 100M, 2, Guid.NewGuid())
                });
            sut.Store(bookOrder1);

            var results = sut.GetBySupplier("Foo", BookOrderState.New).ToList();

            results.Should().BeEmpty();
        }

        [Fact]
        public void Get_ShouldReturnAllBookOrders()
        {
            var sut = CreateSut();

            var bookOrder1 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.Sent,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 100M, 2, Guid.NewGuid())
                });
            var bookOrder2 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });
            var bookOrder3 = BookOrder.CreateExisting("Baz", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);
            sut.Store(bookOrder3);

            var results = sut.Get().ToList();

            results.Should().NotBeNull();
            results.Count.Should().Be(3);
            results.ShouldBeEquivalentTo(
                new [] { bookOrder1, bookOrder2, bookOrder3});
        }

        [Fact]
        public void Get_WhenNoBookOrdersInDatabase_ShouldReturnEmptyList()
        {
            var sut = CreateSut();
            var results = sut.Get();

            results.Should().BeEmpty();
        }

        [Fact]
        public void Delete_ShouldRemoveAllBookOrders()
        {
            var sut = CreateSut();

            var bookOrder1 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.Sent,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 100M, 2, Guid.NewGuid())
                });
            var bookOrder2 = BookOrder.CreateExisting("Foo", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });
            var bookOrder3 = BookOrder.CreateExisting("Baz", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>()
                {
                    new OrderLine("Line1Book", 200M, 3, Guid.NewGuid())
                });

            sut.Store(bookOrder1);
            sut.Store(bookOrder2);
            sut.Store(bookOrder3);

            // act
            sut.Delete();

            // assert
            var bookOrders = sut.Get();

            bookOrders.Should().BeEmpty();
        }
    }
}
