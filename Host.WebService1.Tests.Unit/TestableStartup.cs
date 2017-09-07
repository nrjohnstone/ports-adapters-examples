using Core.Ports.Persistence;
using NSubstitute;

namespace Host.WebService1.Tests.Unit
{
    internal class TestableStartup : Startup
    {
        public IBookOrderRepository MockBookOrderRepository { get; set; }

        public TestableStartup()
        {
            MockBookOrderRepository = Substitute.For<IBookOrderRepository>();    
        }
        
        protected override void RegisterPersistenceAdapter()
        {
            Container.RegisterSingleton<IBookOrderRepository>(MockBookOrderRepository);
        }
    }
}