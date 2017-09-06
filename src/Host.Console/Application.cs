using System;
using System.Linq;
using System.Threading;
using Adapter.Persistence.MySql;
using Adapter.Trigger.Test;
using Core.Entities;
using Core.Ports.Persistence;
using Core.UseCases;
using SimpleInjector;


namespace Host.Console
{
    public class Application : IDisposable
    {
        private readonly Settings _settings;
        protected Container Container;

        private Thread _threadApproveBookOrders;
        private Thread _threadSendBookOrders;

        private Action _triggerAdapterShutdown = () => { };
        private Action _notificationAdapterShutdown = () => { };

        private Action<OrderBookUseCase> _triggerAdapterHandleOrderBookUseCase = (usecase) => { };
        private bool _shutdown;

        public Application(Settings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
            Container = new Container();
        }

        public void Configure()
        {
            Container.Register<OrderBookUseCase>();
            Container.Register<ApproveBookOrderUseCase>();
            Container.Register<SendBookOrderUseCase>();            

            ConfigurePersistenceAdapter();
            ConfigureNotificationAdapter();
            ConfigureTriggerAdapter();            
        }

        private void ConfigurePersistenceAdapter()
        {
            if (_settings.PersistenceAdapter == "Test")
            {
                var persistenceAdapter = new Adapter.Persistence.Test.PersistenceAdapter();
                persistenceAdapter.Initialize();
                persistenceAdapter.Register(Container);
            }
            else if (_settings.PersistenceAdapter == "MySql")
            {
                var persistenceAdapter = new Adapter.Persistence.MySql.PersistenceAdapter(
                    new PersistenceAdapterSettings()
                    {
                        ConnectionString = _settings.PersistenceConnectionString
                    });

                persistenceAdapter.Initialize();
                persistenceAdapter.Register(Container);
            }
        }

        private void ConfigureTriggerAdapter()
        {
            if (_settings.TriggerAdapter == "Test")
            {
                var triggerAdapter = new TriggerAdapter();
                triggerAdapter.Initialize();
                _triggerAdapterShutdown = () => { triggerAdapter.Shutdown(); };
                _triggerAdapterHandleOrderBookUseCase = (usecase) => { triggerAdapter.Handle(usecase); };
            }
            else if (_settings.TriggerAdapter == "RabbitMq")
            {
                var triggerAdapter = new Adapter.Trigger.RabbitMq.TriggerAdapter();
                triggerAdapter.Initialize();
                _triggerAdapterShutdown = () => { triggerAdapter.Shutdown(); };
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
            var orderBookUseCase = Container.GetInstance<OrderBookUseCase>();
            _triggerAdapterHandleOrderBookUseCase(orderBookUseCase);
            
            _threadApproveBookOrders = new Thread(ApproveBookOrders);
            _threadApproveBookOrders.Start();

            _threadSendBookOrders = new Thread(SendBookOrders);
            _threadSendBookOrders.Start();
        }

        private void SendBookOrders()
        {
            IBookOrderRepository bookOrderRepository = Container.GetInstance<IBookOrderRepository>();
            SendBookOrderUseCase sendBookOrderUseCase = Container.GetInstance<SendBookOrderUseCase>();

            while (!_shutdown)
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

            while (!_shutdown)
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
            _shutdown = true;
            _triggerAdapterShutdown();
            _notificationAdapterShutdown();

            if (!_threadSendBookOrders.Join(TimeSpan.FromSeconds(1)))
                _threadSendBookOrders.Abort();            

            if (!_threadApproveBookOrders.Join(TimeSpan.FromSeconds(1)))
                _threadApproveBookOrders.Abort();
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}