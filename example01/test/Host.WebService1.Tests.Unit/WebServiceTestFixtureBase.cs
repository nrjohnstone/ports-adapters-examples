using System;
using System.Net.Http;
using Domain.Ports.Notification;
using Domain.Ports.Persistence;
using Microsoft.Owin.Testing;

namespace Host.WebService.Client1.Tests.Unit
{
    public class WebServiceTestFixtureBase : IDisposable
    {
        protected IBookOrderRepository MockBookOrderRepository => _startup.MockBookOrderRepository;
        protected IBookSupplierGateway MockBookSupplierGateway => _startup.MockBookSupplierGateway;
        protected TestServer Server { get; private set; }
        protected HttpClient Client { get; private set; }

        public WebServiceTestFixtureBase()
        {
            _startup = new TestableStartup();
        }

        public void StartServer()
        {            
            Server = TestServer.Create(_startup.Configuration);
            Client = Server.HttpClient;
        }

        public void Dispose()
        {
            Server?.Dispose();
            Client?.Dispose();
        }

        private TestableStartup _startup;
    }
}