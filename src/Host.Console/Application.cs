using System.Linq;
using System.Threading;
using Adapter.Command;
using Adapter.Notification.Test;
using Adapter.Persistence.Test;
using Adapter.Trigger;
using Core.Entities;
using Core.Ports.Persistence;
using Core.UseCases;
using SimpleInjector;
using TriggerAdapter = Adapter.Trigger.RabbitMq.TriggerAdapter;

namespace Host.Console
{
    public class Application
    {
        protected Container Container;

        private TriggerAdapter _commandAdapter;
        private Thread _threadApproveBookOrders;
        private Thread _threadSendBookOrders;

        public Application()
        {
            Container = new Container();
        }

        public void Configure()
        {
            //Container.Register<OrderBookUseCase>();

            var persistenceAdapter = new PersistenceAdapter();
            persistenceAdapter.Initialize();
            persistenceAdapter.Register(Container);

            var notificationAdapter = new NotificationAdapter();
            notificationAdapter.Initialize();
            notificationAdapter.Register(Container);

            //_commandAdapter = new TriggerAdapter();
            _commandAdapter = new TriggerAdapter();
            _commandAdapter.Initialize();
        }

        public void Run()
        {
            var orderBookCommand = Container.GetInstance<OrderBookUseCase>();

            _commandAdapter.Handle(orderBookCommand);

            _threadApproveBookOrders = new Thread(ApproveBookOrders);
            _threadApproveBookOrders.Start();

            _threadSendBookOrders = new Thread(SendBookOrders);
            _threadSendBookOrders.Start();
        }

        private void SendBookOrders()
        {
            IBookOrderRepository bookOrderRepository = Container.GetInstance<IBookOrderRepository>();
            SendBookOrderUseCase sendBookOrderUseCase = Container.GetInstance<SendBookOrderUseCase>();

            while (true)
            {
                var bookOrderToSend = bookOrderRepository.GetByState(BookOrderState.Approved).FirstOrDefault();

                if (bookOrderToSend != null)
                {
                    sendBookOrderUseCase.Execute(bookOrderToSend.Id);
                }

                Thread.Sleep(1000);
            }
        }

        private void ApproveBookOrders()
        {
            IBookOrderRepository bookOrderRepository = Container.GetInstance<IBookOrderRepository>();
            ApproveBookOrderUseCase approveBookOrderUseCase = Container.GetInstance<ApproveBookOrderUseCase>();

            while (true)
            {
                var bookOrderToApprove = bookOrderRepository.GetByState(BookOrderState.New).FirstOrDefault();

                if (bookOrderToApprove != null)
                {
                    approveBookOrderUseCase.Execute(bookOrderToApprove.Id);
                }

                Thread.Sleep(1000);
            }
        }

        public void Shutdown()
        {
            _commandAdapter.Shutdown();
            _threadSendBookOrders.Abort();            
            _threadApproveBookOrders.Abort();

            _threadSendBookOrders.Join();
            _threadApproveBookOrders.Join();
        }
    }
}