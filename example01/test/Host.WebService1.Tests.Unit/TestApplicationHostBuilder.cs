using Adapter.Persistence.InMemory;
using Domain.Ports.Notification;
using Domain.Ports.Persistence;
using HostApp.WebService.Client1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using SimpleInjector;

namespace HostApp.WebService.Client1.Tests.Unit
{
    internal class TestApplicationHostBuilder : ApplicationHostBuilder
    {
        public IBookOrderRepository BookOrderRepository { get; set; }
        public IBookSupplierGateway MockBookSupplierGateway { get; set; }
        public IBookOrderLineConflictRepository MockBookOrderLineConflictRepository { get; set; }
        
        public TestApplicationHostBuilder(string[] args, string applicationName, Container container) : base(args, applicationName, container)
        {
            BookOrderRepository = new BookOrderRepositoryInMemory();
            MockBookSupplierGateway = Substitute.For<IBookSupplierGateway>();
            MockBookOrderLineConflictRepository = Substitute.For<IBookOrderLineConflictRepository>();
            
            container.Options.AllowOverridingRegistrations = true;
        }

        protected override void PreHostBuildActions(IHostBuilder hostBuilder, Container container)
        {
            container.RegisterInstance<IBookOrderRepository>(BookOrderRepository);
            container.RegisterInstance<IBookOrderLineConflictRepository>(MockBookOrderLineConflictRepository);
            container.RegisterInstance<IBookSupplierGateway>(MockBookSupplierGateway);
            
            hostBuilder.ConfigureWebHost(webHost => webHost.UseTestServer());
        }
    }
}