using System;
using System.Net.Http;
using Domain.Ports.Notification;
using Domain.Ports.Persistence;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using SimpleInjector;

namespace Host.WebService.Client1.Tests.Unit
{
    public class WebServiceTestFixtureBase : IDisposable
    {
        protected IBookOrderRepository BookOrderRepository => _applicationHostBuilder.BookOrderRepository;
        protected IBookOrderLineConflictRepository MockBookOrderLineRepositry => _applicationHostBuilder.MockBookOrderLineConflictRepository;
        protected IBookSupplierGateway MockBookSupplierGateway => _applicationHostBuilder.MockBookSupplierGateway;
        
        protected HttpClient Client { get; private set; }

        public WebServiceTestFixtureBase()
        {
            Container container = new Container();
            
            _applicationHostBuilder = new TestApplicationHostBuilder(new []{ "" }, "Host.WebService.Client1.Tests.Unit", container);
        }

        public void StartServer()
        {
            _host = _applicationHostBuilder.Build();

            _host.StartAsync().GetAwaiter().GetResult();

            Client = _host.GetTestClient();
        }

        public async void Dispose()
        {
            Client?.Dispose();
            await _host.StopAsync();
            _host.Dispose();
        }

        private TestApplicationHostBuilder _applicationHostBuilder;
        private IHost _host;
    }
}