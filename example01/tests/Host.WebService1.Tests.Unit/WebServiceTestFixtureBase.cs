using System;
using System.Net.Http;
using Adapter.Notification.InMemory;
using Adapter.Persistence.InMemory;
using Domain.Ports.Notification;
using Domain.Ports.Persistence;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using SimpleInjector;

namespace HostApp.WebService.Client1.Tests.Unit
{
    public class WebServiceTestFixtureBase : IDisposable
    {
        protected BookOrderRepositoryInMemory BookOrderRepositoryInMemory => _applicationHostBuilder.BookOrderRepositoryInMemory;
        protected BookSupplierGatewayInMemory BookSupplierGatewayInMemory => _applicationHostBuilder.BookSupplierGatewayInMemory;

        /// <summary>
        /// Allow tests to override any default registrations eg. replace InMemory instance with a mock for throwing exceptions
        /// or add extra ones
        /// </summary>
        protected Action<Container> TestContainerRegistrations = container => { };
            
        protected HttpClient Client { get; private set; }

        public WebServiceTestFixtureBase()
        {
            Container container = new Container();
            
            _applicationHostBuilder = new TestApplicationHostBuilder(new []{ "" }, "HostApp.WebService.Client1.Tests.Unit", container);
        }

        public void StartServer()
        {
            _applicationHostBuilder.TestContainerRegistrations = TestContainerRegistrations;
            
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