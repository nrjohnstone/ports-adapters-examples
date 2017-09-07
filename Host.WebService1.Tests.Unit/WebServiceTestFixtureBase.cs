using System;
using System.Net.Http;
using Microsoft.Owin.Testing;

namespace Host.WebService1.Tests.Unit
{
    public class WebServiceTestFixtureBase : IDisposable
    {
        protected TestServer Server { get; private set; }
        protected HttpClient Client { get; private set; }

        public void StartServer()
        {
            var startup = new Startup();
            Server = TestServer.Create(startup.Configuration);
            Client = Server.HttpClient;
        }

        public void Dispose()
        {
            Server?.Dispose();
            Client?.Dispose();
        }
    }
}