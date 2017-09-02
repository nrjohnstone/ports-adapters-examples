using Adapter.Command;
using Adapter.Persistence.Test;
using Core.UseCases;
using SimpleInjector;

namespace Host.Console
{
    public class Application
    {
        protected Container Container;

        private TriggerAdapter _commandAdapter;

        public Application()
        {
            Container = new Container();
        }

        public void Configure()
        {
            Container.Register<OrderBookUseCase>();

            var persistenceAdapter = new PersistenceAdapter();
            persistenceAdapter.Initialize();
            persistenceAdapter.Register(Container);

            _commandAdapter = new TriggerAdapter();
            _commandAdapter.Initialize();
        }

        public void Run()
        {
            var orderBookCommand = Container.GetInstance<OrderBookUseCase>();

            _commandAdapter.Handle(orderBookCommand);
        }

        public void Shutdown()
        {
            _commandAdapter.Shutdown();
        }
    }
}