using System;
using System.Linq;
using System.Threading;
using Adapter.Notification.InMemory;
using Adapter.Persistence.MySql;
using Adapter.Trigger.Test;
using Domain.Entities;
using Domain.Ports.Persistence;
using Domain.UseCases;
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
        
        private bool _shutdown;

        public Application(Settings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
            Container = new Container();
        }

        public void Configure()
        {
            Container.Register<AddBookTitleRequestUseCase>();
            Container.Register<ApproveBookOrderUseCase>();
            Container.Register<SendBookOrderUseCase>();            

            ConfigurePersistenceAdapter();
            ConfigureNotificationAdapter();
            ConfigureTriggerAdapter();
            ConfigureHostAdapter();
        }

        /// <summary>
        /// Wire upstream ports into host implementations, in this case
        /// some private methods running on separate threads
        /// </summary>
        private void ConfigureHostAdapter()
        {
            _threadApproveBookOrders = new Thread(ApproveBookOrders);
            _threadSendBookOrders = new Thread(SendBookOrders);
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

                // Wire upstream ports into adapter
                triggerAdapter.Handle(Container.GetInstance<AddBookTitleRequestUseCase>());                
            }
            else if (_settings.TriggerAdapter == "RabbitMq")
            {
                var triggerAdapter = new Adapter.Trigger.RabbitMq.TriggerAdapter();
                triggerAdapter.Initialize();
                _triggerAdapterShutdown = () => { triggerAdapter.Shutdown(); };

                // Wire upstream ports into adapter
                triggerAdapter.Handle(Container.GetInstance<AddBookTitleRequestUseCase>());                
            }
        }

        private void ConfigureNotificationAdapter()
        {
            if (_settings.NotificationAdapter == "Test")
            {
                var notificationAdapter = new NotificationAdapter();
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
            else if (_settings.NotificationAdapter == "Email")
            {
                Adapter.Notification.Email.NotificationAdapterSettings settings =
                    new Adapter.Notification.Email.NotificationAdapterSettings(
                        "localhost", 1025, bookSupplierEmail: "BookSupplierGateway@fakedomain.com");
                var notificationAdapter = new Adapter.Notification.Email.NotificationAdapter(settings);
                notificationAdapter.Initialize();
                notificationAdapter.Register(Container);
                _notificationAdapterShutdown = () => notificationAdapter.Shutdown();
            }
        }

        public void Run()
        {
            _threadApproveBookOrders.Start();
            _threadSendBookOrders.Start();
        }

        private void SendBookOrders()
        {
            IBookOrderRepository bookOrderRepository = Container.GetInstance<IBookOrderRepository>();
            SendBookOrderUseCase sendBookOrderUseCase = Container.GetInstance<SendBookOrderUseCase>();
            Random rand = new Random();

            while (!_shutdown)
            {
                var bookOrderToSend = bookOrderRepository.GetByState(BookOrderState.Approved).FirstOrDefault();

                if (bookOrderToSend != null)
                {
                    sendBookOrderUseCase.Execute(bookOrderToSend.Id);
                }

                Thread.Sleep(rand.Next(1000, 10000));
            }
        }

        private void ApproveBookOrders()
        {
            IBookOrderRepository bookOrderRepository = Container.GetInstance<IBookOrderRepository>();
            ApproveBookOrderUseCase approveBookOrderUseCase = Container.GetInstance<ApproveBookOrderUseCase>();
            Random rand = new Random();

            while (!_shutdown)
            {
                var bookOrderToApprove = bookOrderRepository.GetByState(BookOrderState.New).FirstOrDefault();

                if (bookOrderToApprove != null)
                {
                    approveBookOrderUseCase.Execute(bookOrderToApprove.Id);
                }

                Thread.Sleep(rand.Next(1000, 10000));
            }
        }

        public void Shutdown()
        {
            _shutdown = true;
    
            if (!_threadSendBookOrders.Join(TimeSpan.FromSeconds(1)))
                _threadSendBookOrders.Abort();            

            if (!_threadApproveBookOrders.Join(TimeSpan.FromSeconds(1)))
                _threadApproveBookOrders.Abort();

            _triggerAdapterShutdown();
            _notificationAdapterShutdown();
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}