using System;
using Adapter.Notification.InMemory;
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
        public BookOrderRepositoryInMemory BookOrderRepositoryInMemory { get; set; }
        public BookSupplierGatewayInMemory BookSupplierGatewayInMemory { get; set; }
        public IBookOrderLineConflictRepository MockBookOrderLineConflictRepository { get; set; }
        
        public Action<Container> TestContainerRegistrations = container => { };
        
        public TestApplicationHostBuilder(string[] args, string applicationName, Container container) : base(args, applicationName, container)
        {
            BookOrderRepositoryInMemory = new BookOrderRepositoryInMemory();
            BookSupplierGatewayInMemory = new BookSupplierGatewayInMemory();
            MockBookOrderLineConflictRepository = Substitute.For<IBookOrderLineConflictRepository>();
            
            container.Options.AllowOverridingRegistrations = true;
        }

        protected override void PreHostBuildActions(IHostBuilder hostBuilder, Container container)
        {
            // Default registrations for all tests to use
            container.RegisterInstance<IBookOrderRepository>(BookOrderRepositoryInMemory);
            container.RegisterInstance<IBookOrderLineConflictRepository>(MockBookOrderLineConflictRepository);
            container.RegisterInstance<IBookSupplierGateway>(BookSupplierGatewayInMemory);

            // Allow tests to override or add registrations if they wish
            TestContainerRegistrations(container);
            
            hostBuilder.ConfigureWebHost(webHost => webHost.UseTestServer());
        }
    }
}