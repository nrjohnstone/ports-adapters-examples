using System;
using System.Linq;
using System.Threading;
using Adapter.Command;
using Adapter.Notification.Test;
using Adapter.Persistence.MySql;
using Adapter.Trigger;
using Core.Entities;
using Core.Ports.Persistence;
using Core.UseCases;
using SimpleInjector;
using PersistenceAdapter = Adapter.Persistence.Test.PersistenceAdapter;

namespace Host.Console
{
    public class Application : IDisposable
    {
        private readonly Settings _settings;
        protected Container Container;

        private TriggerAdapter _triggerAdapter;
        private Thread _threadApproveBookOrders;
        private Thread _threadSendBookOrders;

        private Action _triggerAdapterShutdown = () => { };
        private Action _notificationAdapterShutdown = () => { };

        private Action<OrderBookUseCase> _triggerAdapterHandleOrderBookUseCase = (usecase) => { };

        public Application(Settings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
            Container = new Container();
        }

        public void Configure()
        {
            Container.Register<SendBookOrderUseCase>();

            ConfigurePersistenceAdapter();
            ConfigureNotificationAdapter();
            ConfigureTriggerAdapter();            
        }

        private void ConfigurePersistenceAdapter()
        {
            if (_settings.PersistenceAdapter == "Test")
            {
                var persistenceAdapter = new PersistenceAdapter();
                persistenceAdapter.Initialize();
                persistenceAdapter.Register(Container);
            }
            else if (_settings.PersistenceAdapter == "MySql")
            {
                var persistenceAdapter = new Adapter.Persistence.MySql.PersistenceAdapter(
                    new PersistenceAdapterSettings()
                    {
                        ConnectionString = _settings.ConnectionString
                    });

                persistenceAdapter.Initialize();
                persistenceAdapter.Register(Container);
            }
        }

        private void ConfigureTriggerAdapter()
        {
            if (_settings.TriggerAdapter == "Test")
            {
                var triggerAdapter = new Adapter.Command.TriggerAdapter();
                triggerAdapter.Initialize();
                _triggerAdapterHandleOrderBookUseCase = (usecase) => { triggerAdapter.Handle(usecase); };
            }
            else if (_settings.TriggerAdapter == "RabbitMq")
            {
                var triggerAdapter = new Adapter.Trigger.RabbitMq.TriggerAdapter();
                triggerAdapter.Initialize();
                _triggerAdapterHandleOrderBookUseCase = (usecase) => { triggerAdapter.Handle(usecase); };
            }
        }

        private void ConfigureNotificationAdapter()
        {
            if (_settings.NotificationAdapter == "Test")
            {
                var notificationAdapter = new Adapter.Notification.Test.NotificationAdapter();
                notificationAdapter.Initialize();
                notificationAdapter.Register(Container);
                _notificationAdapterShutdown = () => { };
            }
            else if (_settings.NotificationAdapter == "RabbitMq")
            {
                var notificationAdapter = new Adapter.Notification.RabbitMq.NotificationAdapter();
                notificationAdapter.Initialize();
                notificationAdapter.Register(Container);
                _notificationAdapterShutdown = () => notificationAdapter.Shutdown();
            }
        }

        public void Run()
        {
            var orderBookCommand = Container.GetInstance<OrderBookUseCase>();

            _triggerAdapterHandleOrderBookUseCase(orderBookCommand);
            
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
            _triggerAdapterShutdown();
            _notificationAdapterShutdown();

            _threadSendBookOrders.Abort();            
            _threadApproveBookOrders.Abort();

            _threadSendBookOrders.Join();
            _threadApproveBookOrders.Join();
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}