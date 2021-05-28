using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Domain.Entities;
using Domain.Ports.Persistence;
using FluentAssertions;
using HostApp.WebService.Client1.Tests.Unit.Dtos;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Extensions;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace HostApp.WebService.Client1.Tests.Unit
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
        public async void Post_BookRequest_WhenOrderDoesNotExistForSupplier_ShouldCreateNewBookOrder()
        {
            StartServer();

            var jsonObject = JsonConvert.SerializeObject(new
            {
                Title = "The Maltese Falcon",
                Supplier = "Test",
                Price = 25.50,
                Quantity = 1
            });
            
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            
            // act
            HttpResponseMessage response = await Client.PostAsync("bookRequests", stringContent);

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var storedBookOrders = BookOrderRepositoryInMemory.GetBySupplier("Test").ToList();
            storedBookOrders.Should().NotBeEmpty();
            storedBookOrders.Count().Should().Be(1);
            storedBookOrders[0].Supplier.Should().Be("Test");
            storedBookOrders[0].State.Should().Be(BookOrderState.New);
            storedBookOrders[0].OrderLines[0].Title.Should().Be("The Maltese Falcon");
            storedBookOrders[0].OrderLines[0].Price.Should().Be(25.50M);
            storedBookOrders[0].OrderLines[0].Quantity.Should().Be(1);
        }

        [Fact]
        public async void Post_BookRequest_WhenNewOrderExistsForSupplier_ShouldAddRequestToExistingSupplierOrder()
        {
            StartServer();

            var bookRequest1 = JsonConvert.SerializeObject(new
            {
                Title = "The Maltese Falcon",
                Supplier = "Test",
                Price = 25.50,
                Quantity = 1
            });
            
            // add initial book for the supplier
            var stringContent = new StringContent(bookRequest1, Encoding.UTF8, "application/json");
            HttpResponseMessage response1 = await Client.PostAsync("bookRequests", stringContent);
            
            var bookRequest2 = JsonConvert.SerializeObject(new
            {
                Title = "Gone With the Wind",
                Supplier = "Test",
                Price = 30.50,
                Quantity = 2
            });
            stringContent = new StringContent(bookRequest2, Encoding.UTF8, "application/json");
            
            // act
            HttpResponseMessage response2 = await Client.PostAsync("bookRequests", stringContent);
            
            // assert
            response1.StatusCode.Should().Be(HttpStatusCode.Created);
            response2.StatusCode.Should().Be(HttpStatusCode.Created);

            response1.Headers.Location.ToString().Should().StartWith("bookOrders");
            response2.Headers.Location.ToString().Should().StartWith("bookOrders");
            
            var storedBookOrders = BookOrderRepositoryInMemory.GetBySupplier("Test").ToList();
            storedBookOrders.Should().NotBeEmpty();
            storedBookOrders.Count().Should().Be(1);
            storedBookOrders[0].Supplier.Should().Be("Test");
            storedBookOrders[0].State.Should().Be(BookOrderState.New);
            storedBookOrders[0].OrderLines[0].Title.Should().Be("The Maltese Falcon");
            storedBookOrders[0].OrderLines[0].Price.Should().Be(25.50M);
            storedBookOrders[0].OrderLines[0].Quantity.Should().Be(1);
            storedBookOrders[0].OrderLines[1].Title.Should().Be("Gone With the Wind");
            storedBookOrders[0].OrderLines[1].Price.Should().Be(30.50M);
            storedBookOrders[0].OrderLines[1].Quantity.Should().Be(2);
        }
        
        [Fact]
        public void Post_ApproveBookOrder_WhenBookOrderIsNew_ShouldApproveBookOrder()
        {
            BookOrder bookOrder = BookOrder.CreateNew("SupplierFoo", Guid.NewGuid());
            bookOrder.State.Should().Be(BookOrderState.New);
            BookOrderRepositoryInMemory.Store(bookOrder);
            
            StartServer();
            
            // act
            var result = Client.Post($"bookOrders/{bookOrder.Id}/approve", null);

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var storedBookOrder = BookOrderRepositoryInMemory.Get(bookOrder.Id);
            storedBookOrder.State.Should().Be(BookOrderState.Approved);
        }

        [Fact]
        public void Post_SendBookOrder_WhenBookOrderIsApproved_ShouldSendBookOrder()
        {
            BookOrder bookOrder = BookOrder.CreateNew("SupplierFoo", Guid.NewGuid());
            bookOrder.Approve();
            BookOrderRepositoryInMemory.Store(bookOrder);

            StartServer();

            // act
            var result = Client.Post($"bookOrders/{bookOrder.Id}/send", null);

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var storedBookOrder = BookOrderRepositoryInMemory.Get(bookOrder.Id);
            storedBookOrder.State.Should().Be(BookOrderState.Sent);
            
            BookSupplierGatewayInMemory.SentBookOrders.Count().Should().Be(1);
            BookSupplierGatewayInMemory.SentBookOrders[0].Id.Should().Be(bookOrder.Id);
            BookSupplierGatewayInMemory.SentBookOrders[0].State.Should().Be(BookOrderState.Sent);
        }
        
        /// <summary>
        /// This test demonstrates how to override the base InMemory implementations with a Mock
        /// if you have need to (for example to throw exceptions etc..)
        /// </summary>
        [Fact]
        public void Post_SendBookOrder_ExampleOfOverrideWithMock_ShouldSucceed()
        {
            BookOrder bookOrder = BookOrder.CreateNew("SupplierFoo", Guid.NewGuid());
            bookOrder.Approve();
            
            IBookOrderRepository mockBookOrderRepository = Substitute.For<IBookOrderRepository>();
            mockBookOrderRepository.Get(bookOrder.Id).Returns(bookOrder);
            
            TestContainerRegistrations = container =>
            {
                container.RegisterInstance(mockBookOrderRepository);
            };
            
            StartServer();

            // act
            var result = Client.Post($"bookOrders/{bookOrder.Id}/send", null);

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            mockBookOrderRepository.Received(1).Store(Arg.Is<BookOrder>(
                x => x.Id == bookOrder.Id &&
                     x.State == BookOrderState.Sent));
        }

        [Fact]
        public void Get_ShouldReturnAllBookOrders()
        {
            var bookOrders = new List<BookOrder>();

            var line1 = new OrderLine("Title1", 10.5M, 1, Guid.NewGuid());

            var bookOrder1 = a.BookOrder.ForSupplier("Supplier1")
                .WithId(Guid.NewGuid())
                .WithLine(line1)
                .ThatIsNew();

            bookOrders.Add(bookOrder1);

            var line2 = new OrderLine("Title2", 20.5M, 2, Guid.NewGuid());
            var bookOrder2 = a.BookOrder.ForSupplier("Supplier2")
                .WithId(Guid.NewGuid())
                .WithLine(line2)
                .ThatIsApproved();

            bookOrders.Add(bookOrder2);
            
            BookOrderRepositoryInMemory.Store(bookOrder1);
            BookOrderRepositoryInMemory.Store(bookOrder2);

            StartServer();

            // act
            var result = Client.GetAsync("bookOrders").Result;

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookOrdersResponse =
                JsonConvert.DeserializeObject<IEnumerable<BookOrderResponseDto>>(result.Content.ReadAsStringAsync().Result)
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
            bookOrdersResponse[1].OrderLines.Count.Should().Be(1);
            bookOrdersResponse[1].OrderLines[0].Title.Should().Be("Title2");
            bookOrdersResponse[1].OrderLines[0].Price.Should().Be(20.5M);
            bookOrdersResponse[1].OrderLines[0].Quantity.Should().Be(2);
        }
    }
}