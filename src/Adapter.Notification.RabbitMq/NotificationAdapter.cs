using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Ports.Notification;
using RabbitMQ.Client;
using SimpleInjector;

namespace Adapter.Notification.RabbitMq
{
    public class NotificationAdapter
    {
        private readonly Container _adapterContainer;
        private IConnection _connection;
        private bool _initialized;

        public NotificationAdapter()
        {
            _adapterContainer = new Container();
        }

        public void Initialize()
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            _connection = factory.CreateConnection();

            DeclareExchange();

            _adapterContainer.RegisterSingleton(_connection);
            _adapterContainer.Register<IBookSupplierGateway, BookSupplierGateway>();

            _initialized = true;
        }

        public void Register(Container container)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            container.Register<IBookSupplierGateway>(() => _adapterContainer.GetInstance<IBookSupplierGateway>());
        }

        private void DeclareExchange()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(RabbitMqConstants.SupplierExchangeName, ExchangeType.Topic,
                    durable: false, autoDelete: true);
            }
        }
        
        public void Shutdown()
        {
            _connection?.Dispose();
            _adapterContainer?.Dispose();
        }
    }
}
