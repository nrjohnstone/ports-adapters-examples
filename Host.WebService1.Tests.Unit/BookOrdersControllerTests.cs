using System;
using System.Net;
using Core.Entities;
using FluentAssertions;
using Microsoft.Owin;
using NSubstitute;
using Xunit;

namespace Host.WebService1.Tests.Unit
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
            MockBookOrderRepository.Get(bookOrderId).Returns(new BookOrder(
                "SupplierFoo", bookOrderId, BookOrderState.New));

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
            MockBookOrderRepository.Get(bookOrderId).Returns(new BookOrder(
                "SupplierFoo", bookOrderId, BookOrderState.Approved));

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
    }
}