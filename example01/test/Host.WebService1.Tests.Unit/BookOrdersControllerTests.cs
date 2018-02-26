using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Domain.Entities;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Host.WebService.Client1.Tests.Unit
{
    public class BookOrdersControllerTests : WebServiceTestFixtureBase
    {
        [Fact]
        public void HealthCheck_ShouldReturnOk()
        {
            StartServer();
            var result = Client.GetAsync("health/instance").Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void Post_BookRequest_ShouldCreateNewBookOrder()
        {
            StartServer();

            var result = Client.Post("bookRequests", new
            {
                Title = "The Maltese Falcon",
                Supplier = "Test",
                Price = "25.50",
                Quantity = 1
            });

            result.StatusCode.Should().Be(HttpStatusCode.Created);

            MockBookOrderRepository.Received(1).Store(
                Arg.Is<BookOrder>(
                    x => x.Id != Guid.Empty &&
                    x.Supplier.Equals("Test") &&
                    x.OrderLines[0].Title.Equals("The Maltese Falcon") &&
                    x.OrderLines[0].Price == 25.5M &&
                    x.OrderLines[0].Quantity == 1));
        }

        [Fact]
        public void Post_ApproveBookOrder_WhenBookOrderIsNew_ShouldApproveBookOrder()
        {
            Guid bookOrderId = Guid.NewGuid();
            MockBookOrderRepository.Get(bookOrderId).Returns(BookOrder.CreateNew(
                "SupplierFoo", bookOrderId));

            StartServer();

            var result = Client.Post($"bookOrders/{bookOrderId}/approve", null);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            MockBookOrderRepository.Received(1).Store(
                Arg.Is<BookOrder>(
                    x => x.Id == bookOrderId &&
                    x.State == BookOrderState.Approved));
        }

        [Fact]
        public void Post_SendBookOrder_WhenBookOrderIsApproved_ShouldSendBookOrder()
        {
            Guid bookOrderId = Guid.NewGuid();
            MockBookOrderRepository.Get(bookOrderId).Returns(BookOrder.CreateExisting(
                "SupplierFoo", bookOrderId, BookOrderState.Approved, new List<OrderLine>()));

            StartServer();

            var result = Client.Post($"bookOrders/{bookOrderId}/send", null);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            MockBookOrderRepository.Received(1).Store(
                Arg.Is<BookOrder>(
                    x => x.Id == bookOrderId &&
                         x.State == BookOrderState.Sent));
            MockBookSupplierGateway.Received(1).Send(
                Arg.Is<BookOrder>(
                    x => x.Id == bookOrderId &&
                    x.State == BookOrderState.Sent));
        }

        [Fact]
        public void Get_ShouldReturnAllBookOrders()
        {
            var bookOrders = new List<BookOrder>();
            var bookOrder = BookOrder.CreateExisting("Supplier1", Guid.NewGuid(), BookOrderState.New,
                new List<OrderLine>() { new OrderLine("Title1", 10.5M, 1, Guid.NewGuid())});

            bookOrders.Add(bookOrder);
            bookOrders.Add(BookOrder.CreateExisting("Supplier2", Guid.NewGuid(), BookOrderState.Approved,
                new List<OrderLine>()));

            MockBookOrderRepository.Get().Returns(bookOrders);

            StartServer();

            var result = Client.GetAsync("bookOrders").Result;

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookOrdersResponse =
                JsonConvert.DeserializeObject<IEnumerable<BookOrderResponse>>(result.Content.ReadAsStringAsync().Result)
                    .ToList();

            bookOrdersResponse.Count.Should().Be(2);
            bookOrdersResponse[0].Supplier.Should().Be("Supplier1");
            bookOrdersResponse[0].State.Should().Be("New");
            bookOrdersResponse[0].Id.Should().Be(bookOrders[0].Id.ToString());
            bookOrdersResponse[0].OrderLines.Count.Should().Be(1);
            bookOrdersResponse[0].OrderLines[0].Title.Should().Be("Title1");
            bookOrdersResponse[0].OrderLines[0].Price.Should().Be(10.5M);
            bookOrdersResponse[0].OrderLines[0].Quantity.Should().Be(1);
            bookOrdersResponse[1].Supplier.Should().Be("Supplier2");
            bookOrdersResponse[1].State.Should().Be("Approved");
            bookOrdersResponse[1].Id.Should().Be(bookOrders[1].Id.ToString());
        }
    }

    internal class BookOrderResponse
    {
        public string Supplier { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public IList<OrderLineResponse> OrderLines { get; set; }
    }

    internal class OrderLineResponse
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}