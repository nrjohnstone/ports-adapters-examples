using Core.Ports.Notification;
using Core.Ports.Persistence;
using NSubstitute;

namespace Host.WebService.Client1.Tests.Unit
{
    /// <summary>
    /// Startup class for testing that derives from the real startup class but provides
    /// seams for injecting testable dependencies for ports where required
    /// </summary>
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