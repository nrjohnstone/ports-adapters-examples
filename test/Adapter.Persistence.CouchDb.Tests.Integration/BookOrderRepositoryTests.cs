using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Adapter.Persistence.CouchDb.Repositories;
using Domain.Entities;
using FluentAssertions;
using MyCouch;
using Xunit;

namespace Adapter.Persistence.CouchDb.Tests.Integration
{
    public class BookOrderRepositoryTests
    {
        public BookOrderRepositoryTests()
        {
            CreateDatabaseIfNotExists();
        }

        private static void CreateDatabaseIfNotExists()
        {
            using (var couchDb = new MyCouchServerClient("http://admin:123@localhost:5984"))
            {
                if (couchDb.Databases.GetAsync("mydb").Result.StatusCode == HttpStatusCode.OK)
                    return;

                var response = couchDb.Databases.PutAsync("mydb");

                response.Result.StatusCode.Should().Be(HttpStatusCode.Created);
            }
        }

        private BookOrderRepository CreateSut()
        {
            return new BookOrderRepository();
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

            var bookOrder = new BookOrder("AcmeBooks", bookOrderId, BookOrderState.New,
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

            var bookOrder = new BookOrder("AcmeBooks", bookOrderId, BookOrderState.New,
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
    }
}
