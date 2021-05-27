using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Flurl;
using HostApp.WebService.Client2.Tests.Acceptance.Dtos;
using RabbitMQ.Client;
using RestSharp;

namespace HostApp.WebService.Client2.Tests.Acceptance.Scenarios
{
    /// <summary>
    /// Validate the happy path for Client 2
    /// <br/> * Create a new book order by sending a book title request from a supplier that does not already have a new order
    /// <br/> * Add to an existing "new" book order by sending a book title request from a supplier with an existing "new" book order
    /// <br/> * Create a second book order by sending a book title request from a different supplier
    /// <br/> * Approve a book order that is in the New state
    /// <br/> * Send a book order that is in the Approved state
    /// </summary>
    internal class Scenario1 : ScenarioBase
    {
        private string _supplier1Name;
        private string _supplier2Name;

        public Scenario1(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        
        public void Run()
        {
            try
            {
                GenerateRandomSupplierNames();
                RequestBookTitleRequestForSupplier1();
                VerifyNewBookOrderExists();
                RequestNextBookTitleRequestForSupplier1();
                VerifyBookOrderUpdated();
                
                RequestBookTitleRequestForSupplier2();
                VerifyNewBookOrderExistsForSupplier2();
                VerifyNewBookOrderForSupplier1NotChanged();

                ApproveBookOrderForSupplier1();
                VerifyBookOrderForSupplier1IsApproved();

                ConnectToSupplierQueue();
                SendBookOrderForSupplier1();
                VerifyBookOrderForSupplier1IsSent();
                VerifyBookOrderMessageForSupplier1IsSent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TestFailed();
            }

            TestPassed();
        }
        
        /// <summary>
        /// Need to create a queue and attach it to the correct exchange with the right routing key to obtain the message
        /// that is sent to suppliers when a book is to be ordered
        /// </summary>
        private void ConnectToSupplierQueue()
        {
            InitializeSupplierQueue();
        }

        private void VerifyBookOrderMessageForSupplier1IsSent()
        {
            var supplierBookOrderDto = GetBookOrderMessageFromSupplierQueue();
            
            // Assert that the book order message on the supplier queue has the expected values
            supplierBookOrderDto.Id.Should().NotBeEmpty();
            supplierBookOrderDto.Supplier.Should().Be(_supplier1Name);
            supplierBookOrderDto.OrderLines.Count().Should().Be(2);
            supplierBookOrderDto.OrderLines.Should().ContainSingle(
                x => x.Title == "The Matrix" &&
                     x.Price == 20 &&
                     x.Quantity == 2);
            supplierBookOrderDto.OrderLines.Should().ContainSingle(
                x => x.Title == "A Warm Summers Evening" &&
                     x.Price == 10 &&
                     x.Quantity == 1);
            supplierBookOrderDto.State.Should().Be("Sent");
        }

        private void VerifyBookOrderForSupplier1IsSent()
        {
            var bookOrder = GetBookOrderForSupplier(_supplier1Name);
            bookOrder.State.Should().Be("Sent");
            
            // Verify the book order lines still remain as expected
            bookOrder.OrderLines.Count().Should().Be(2);
      
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "The Matrix" &&
                     x.Price == 20 &&
                     x.Quantity == 2);
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "A Warm Summers Evening" &&
                     x.Price == 10 &&
                     x.Quantity == 1);
        }

        /// <summary>
        /// POST to the REST endpoint of the API to execute the "Send" business logic
        /// for a book order that is approved
        /// </summary>
        private void SendBookOrderForSupplier1()
        {
            RestClient client = GetRestClient();

            var bookOrder = GetBookOrderForSupplier(_supplier1Name);
            
            string uri = "bookOrders"
                .AppendPathSegment(bookOrder.Id)
                .AppendPathSegment("send");
            
            RestRequest request = new RestRequest(uri, Method.POST);
            var response = client.Execute(request);

            response.IsSuccessful.Should().BeTrue();
        }

        /// <summary>
        /// Retrieve the book order for Supplier 1 and verify that the order contains the expected values
        /// and the state of the order is "Approved"
        /// </summary>
        private void VerifyBookOrderForSupplier1IsApproved()
        {
            var bookOrder = GetBookOrderForSupplier(_supplier1Name);
            bookOrder.State.Should().Be("Approved");
            
            // Verify the book order lines still remain as expected
            bookOrder.OrderLines.Count().Should().Be(2);
      
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "The Matrix" &&
                     x.Price == 20 &&
                     x.Quantity == 2);
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "A Warm Summers Evening" &&
                     x.Price == 10 &&
                     x.Quantity == 1);
        }

        /// <summary>
        /// POST to the REST endpoint of the API to execute the "Approve" business logic
        /// for a book order that is new
        /// </summary>
        private void ApproveBookOrderForSupplier1()
        {
            RestClient client = GetRestClient();

            var bookOrder = GetBookOrderForSupplier(_supplier1Name);
            
            string uri = "bookOrders"
                .AppendPathSegment(bookOrder.Id)
                .AppendPathSegment("approve");
            
            RestRequest request = new RestRequest(uri, Method.POST);
            IRestResponse response = client.Execute(request);

            response.IsSuccessful.Should().BeTrue();
        }

        private void VerifyNewBookOrderForSupplier1NotChanged()
        { 
            bool found = false;
            DateTime timeout = DateTime.Now + TimeSpan.FromSeconds(10);

            RestClient client = GetRestClient();
            IRestResponse<List<BookOrderResponseDto>> response = null;
            
            do
            {
                RestRequest request = new RestRequest("bookOrders");

                response = client.Execute<List<BookOrderResponseDto>>(request);
                found = response.Data.Any(x => x.Supplier == _supplier1Name && x.OrderLines.Count() == 2);
                if (!found)
                {
                    Thread.Sleep(200);
                }
            } while (!found && DateTime.Now < timeout);
            
            response.Data.Should().ContainSingle(x => x.Supplier == _supplier1Name);

            var bookOrder = response.Data.Single(x => x.Supplier == _supplier1Name);
            bookOrder.OrderLines.Count().Should().Be(2);
      
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "The Matrix" &&
                     x.Price == 20 &&
                     x.Quantity == 2);
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "A Warm Summers Evening" &&
                     x.Price == 10 &&
                     x.Quantity == 1);
        }

        private void VerifyNewBookOrderExistsForSupplier2()
        {
            bool found = false;
            DateTime timeout = DateTime.Now + TimeSpan.FromSeconds(10);

            RestClient client = GetRestClient();
            IRestResponse<List<BookOrderResponseDto>> response = null;
            
            do
            {
                RestRequest request = new RestRequest("bookOrders");

                response = client.Execute<List<BookOrderResponseDto>>(request);
                found = response.Data.Any(x => x.Supplier == _supplier2Name);
                if (!found)
                {
                    Thread.Sleep(200);
                }
            } while (!found && DateTime.Now < timeout);
            
            response.Data.Should().ContainSingle(x => x.Supplier == _supplier2Name && x.State == "New",
                "A book order for supplier 2 should have been created in the New state");
            
            var bookOrder = response.Data.Single(x => x.Supplier == _supplier2Name);
            bookOrder.OrderLines.Count().Should().Be(1);

            OrderLineResponseDto expectedDto = new OrderLineResponseDto()
            {
                Price = 50M,
                Quantity = 5,
                Title = "The Two Towers"
            };

            bookOrder.OrderLines.Should().ContainEquivalentOf(expectedDto, "A new book order for supplier 2 should have " +
                                                                           "been created with the expected order line");
        }

        private void RequestBookTitleRequestForSupplier2()
        {
            BookTitleRequestDto dto = new BookTitleRequestDto()
            {
                Title = "The Two Towers",
                Price = 50,
                Quantity = 5,
                Supplier = _supplier2Name
            };
            
            SendBookTitleRequest(dto);
        }

        private void VerifyBookOrderUpdated()
        {
            bool found = false;
            DateTime timeout = DateTime.Now + TimeSpan.FromSeconds(10);

            RestClient client = GetRestClient();
            IRestResponse<List<BookOrderResponseDto>> response = null;
            
            do
            {
                RestRequest request = new RestRequest("bookOrders");

                response = client.Execute<List<BookOrderResponseDto>>(request);
                found = response.Data.Any(x => x.Supplier == _supplier1Name && x.OrderLines.Count() == 2);
                if (!found)
                {
                    Thread.Sleep(200);
                }
            } while (!found && DateTime.Now < timeout);
            
            response.Data.Should().ContainSingle(x => x.Supplier == _supplier1Name);

            var bookOrder = response.Data.Single(x => x.Supplier == _supplier1Name);
            bookOrder.OrderLines.Count().Should().Be(2);
      
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "The Matrix" &&
                     x.Price == 20 &&
                     x.Quantity == 2);
            bookOrder.OrderLines.Should().ContainSingle(
                x => x.Title == "A Warm Summers Evening" &&
                     x.Price == 10 &&
                     x.Quantity == 1);
        }

        private void RequestNextBookTitleRequestForSupplier1()
        {
            BookTitleRequestDto dto = new BookTitleRequestDto()
            {
                Title = "The Matrix",
                Price = 20,
                Quantity = 2,
                Supplier = _supplier1Name
            };

            SendBookTitleRequest(dto);
        }

        private void TestFailed()
        {
            Console.WriteLine("TEST FAILED");
        }

        private void TestPassed()
        {
            Console.WriteLine("TEST PASSED");
        }
        
        private void VerifyNewBookOrderExists()
        {
            bool found;
            DateTime timeout = DateTime.Now + TimeSpan.FromSeconds(10);

            RestClient client = GetRestClient();
            IRestResponse<List<BookOrderResponseDto>> response = null;
            
            do
            {
                RestRequest request = new RestRequest("bookOrders");

                response = client.Execute<List<BookOrderResponseDto>>(request);
                found = response.Data.Any(x => x.Supplier == _supplier1Name);
                if (!found)
                {
                    Thread.Sleep(200);
                }
            } while (!found && DateTime.Now < timeout);
            
            response.Data.Should().ContainSingle(x => x.Supplier == _supplier1Name);
        }

        private void GenerateRandomSupplierNames()
        {
            // Create random supplier names so acceptance tests can be re-run without deleting orders
            _supplier1Name = $"Supplier1_{Guid.NewGuid()}";
            _supplier2Name = $"Supplier2_{Guid.NewGuid()}";
        }

        private void RequestBookTitleRequestForSupplier1()
        {
            BookTitleRequestDto dto = new BookTitleRequestDto()
            {
                Title = "A Warm Summers Evening",
                Price = 10,
                Quantity = 1,
                Supplier = _supplier1Name
            };

            SendBookTitleRequest(dto);
        }
    }
}