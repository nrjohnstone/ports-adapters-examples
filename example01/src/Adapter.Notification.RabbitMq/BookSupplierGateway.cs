using System;
using System.Text;
using Domain.Entities;
using Domain.Ports.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace Adapter.Notification.RabbitMq
{
    internal class BookSupplierGateway : IBookSupplierGateway
    {
        private readonly IConnection _connection;

        public BookSupplierGateway(IConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            _connection = connection;
        }

        public void Send(BookOrder bookOrder)
        {
            using (var channel = _connection.CreateModel())
            {
                byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bookOrder, new StringEnumConverter()));
                channel.BasicPublish(RabbitMqConstants.SupplierExchangeName, RabbitMqConstants.SupplierQueueName, null, body);
            }
        }
    }
}