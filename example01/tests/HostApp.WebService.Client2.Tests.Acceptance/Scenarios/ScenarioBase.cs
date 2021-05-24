using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using HostApp.WebService.Client2.Tests.Acceptance.Dtos;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;

namespace HostApp.WebService.Client2.Tests.Acceptance.Scenarios
{
    /// <summary>
    /// Base class for commonly required functionality not specific or related to test scenarios
    /// </summary>
    internal class ScenarioBase
    {
        private readonly ConnectionFactory _connectionFactory;
        
        public ScenarioBase(ConnectionFactory connectionFactory)
        {
            if (connectionFactory == null) throw new ArgumentNullException(nameof(connectionFactory));
            _connectionFactory = connectionFactory;
        }
        
        /// <summary>
        /// Serialize the BookTitleRequestDto and place it on the correct RabbitMq exchange and queue
        /// </summary>
        /// <param name="dto"></param>
        protected void SendBookTitleRequest(BookTitleRequestDto dto)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var model = connection.CreateModel();
                
                var properties = model.CreateBasicProperties();
                properties.Persistent = false;

              
                string json = JsonConvert.SerializeObject(dto);
                
                byte[] messagebuffer = Encoding.Default.GetBytes(json);

                model.BasicPublish("bookorder", "bookrequest", false, properties, messagebuffer);
            }
        }
        
        protected static RestClient GetRestClient()
        {
            RestClient client = new RestClient("http://localhost:10009");
            return client;
        }
        
        protected BookOrderResponseDto GetBookOrderForSupplier(string supplierName)
        {
            var client = GetRestClient();
            IRestResponse<List<BookOrderResponseDto>> response = null;
            RestRequest request = new RestRequest("bookOrders");

            response = client.Execute<List<BookOrderResponseDto>>(request);

            response.IsSuccessful.Should().BeTrue();

            // The tests are designed that there should only ever be a single order for a supplier
            var bookOrderResponseDto = response.Data.Single(x => x.Supplier == supplierName);
            return bookOrderResponseDto;
        }

    }
}