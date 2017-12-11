using System;
using System.Web.Http;
using Adapter.Trigger.RabbitMq;
using Domain.UseCases;
using Host.WebService.Client3.BookOrders;
using Owin;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using Swashbuckle.Application;

namespace Host.WebService.Client3
{
    public class Startup
    {
        protected Container Container;        
        private Action _notificationAdapterShutdown = () => { };
        
        public Startup()
        {
            Container = new Container();            
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            RegisterPersistenceAdapter();
            RegisterNotificationAdapter();
            RegisterControllers();
            RegisterHostAdapter();
            
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);

            config.MapHttpAttributeRoutes();
            config.Routes.IgnoreRoute("IgnoreAxdFiles", "{resource}.axd/{*pathInfo}");

            config.EnableSwagger(c => c.SingleApiVersion("v1", "BookOrders Client 3"))
                .EnableSwaggerUi();

            config.EnsureInitialized();
            
            appBuilder.UseWebApi(config);
        }

        protected virtual void RegisterPersistenceAdapter()
        {
            var persistenceAdapter = new Adapter.Persistence.CouchDb.PersistenceAdapter(
                new Adapter.Persistence.CouchDb.PersistenceAdapterSettings(
                    "http://admin:123@localhost:5984", "bookorders"));

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
            Container.Register<OrderBookUseCase>();
            Container.Register<ApproveBookOrderUseCase>();
            Container.Register<SendBookOrderUseCase>();
            Container.Register<GetBookOrdersUseCase>();
            Container.Register<DeleteBookOrdersUseCase>();
        }

        public void Shutdown()
        {
            _notificationAdapterShutdown();
            Container?.Dispose();
        }
    }
}
