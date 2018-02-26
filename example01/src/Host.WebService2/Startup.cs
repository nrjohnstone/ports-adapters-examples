using System;
using System.Web.Http;
using Adapter.Persistence.MySql;
using Adapter.Trigger.RabbitMq;
using Domain.UseCases;
using Host.WebService.Client2.BookOrders;
using Owin;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using Swashbuckle.Application;

namespace Host.WebService.Client2
{
    public class Startup
    {
        protected Container Container;
        private Action _triggerAdapterShutdown = () => { };
        private Action _notificationAdapterShutdown = () => { };

        private TriggerAdapter _triggerAdapter;

        public Startup()
        {
            Container = new Container();            
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            RegisterPersistenceAdapter();
            RegisterNotificationAdapter();
            RegisterTriggerAdapter();
            RegisterControllers();
            RegisterHostAdapter();

            AttachUseCasesToTriggers();

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);

            config.MapHttpAttributeRoutes();
            config.Routes.IgnoreRoute("IgnoreAxdFiles", "{resource}.axd/{*pathInfo}");

            config.EnableSwagger(c => c.SingleApiVersion("v1", "BookOrders Client 2"))
                .EnableSwaggerUi();

            config.EnsureInitialized();
            
            appBuilder.UseWebApi(config);
        }

        protected virtual void RegisterPersistenceAdapter()
        {
            var persistenceAdapter = new Adapter.Persistence.MySql.PersistenceAdapter(
                new PersistenceAdapterSettings()
                {
                    ConnectionString = "server=127.0.0.1;" +
                                       "uid=bookorder_service;" +
                                       "pwd=123;" +
                                       "database=bookorders"
                });

            persistenceAdapter.Initialize();
            persistenceAdapter.Register(Container);
        }

        protected virtual void RegisterNotificationAdapter()
        {
            var notificationAdapter = new Adapter.Notification.RabbitMq.NotificationAdapter();
            notificationAdapter.Initialize();
            notificationAdapter.Register(Container);
            _notificationAdapterShutdown = () => { notificationAdapter.Shutdown(); };

        }

        private void RegisterTriggerAdapter()
        {
            _triggerAdapter = new Adapter.Trigger.RabbitMq.TriggerAdapter();
            _triggerAdapter.Initialize();
            _triggerAdapterShutdown = () => { _triggerAdapter.Shutdown(); };
        }

        private void RegisterControllers()
        {
            Container.Register<BookOrdersController>();
        }

        /// <summary>
        /// Wire upstream ports to host implementations, in this case
        /// we are registering the ports (use cases) in the IoC container
        /// as the Controllers are the host adapter implementations as they 
        /// call into the use cases
        /// </summary>
        private void RegisterHostAdapter()
        {
            Container.Register<AddBookTitleRequestUseCase>();
            Container.Register<ApproveBookOrderUseCase>();
            Container.Register<SendBookOrderUseCase>();
            Container.Register<GetAllBookOrdersUseCase>();
            Container.Register<DeleteBookOrdersUseCase>();
        }

        private void AttachUseCasesToTriggers()
        {
            // Wire upstream ports into adapter
            _triggerAdapter.Handle(Container.GetInstance<AddBookTitleRequestUseCase>());
        }

        public void Shutdown()
        {
            _notificationAdapterShutdown();
            _triggerAdapterShutdown();
            Container?.Dispose();
        }
    }
}
