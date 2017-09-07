using Core.Ports.Notification;
using Core.Ports.Persistence;
using NSubstitute;

namespace Host.WebService1.Tests.Unit
{
    internal class TestableStartup : Startup
    {
        public IBookOrderRepository MockBookOrderRepository { get; set; }
        public IBookSupplierGateway MockBookSupplierGateway { get; set; }

        public TestableStartup()
        {
            MockBookOrderRepository = Substitute.For<IBookOrderRepository>();
            MockBookSupplierGateway = Substitute.For<IBookSupplierGateway>();
            Container.Options.AllowOverridingRegistrations = true;
        }
        
        protected override void RegisterPersistenceAdapter()
        {
            Container.RegisterSingleton<IBookOrderRepository>(MockBookOrderRepository);
            
        }

        protected override void RegisterNotificationAdapter()
        {
            Container.RegisterSingleton<IBookSupplierGateway>(MockBookSupplierGateway);
        }
    }
}