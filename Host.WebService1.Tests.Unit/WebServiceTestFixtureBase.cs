using System;
using System.Net.Http;
using Core.Ports.Persistence;
using Microsoft.Owin.Testing;

namespace Host.WebService1.Tests.Unit
{
    public class WebServiceTestFixtureBase : IDisposable
    {
        protected IBookOrderRepository MockBookOrderRepository => _startup.MockBookOrderRepository;
        protected TestServer Server { get; private set; }
        protected HttpClient Client { get; private set; }

        public void StartServer()
        {
            _startup = new TestableStartup();
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