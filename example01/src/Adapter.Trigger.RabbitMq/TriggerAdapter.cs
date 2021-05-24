using System;
using Domain.UseCases;
using RabbitMQ.Client;
using static Adapter.Trigger.RabbitMq.RabbitMqConstants;

namespace Adapter.Trigger.RabbitMq
{
    public class TriggerAdapter : IDisposable
    {
        private OrderBookUseCaseTrigger _orderBookUseCaseTrigger;
        private bool _initialized;
        private IConnection _connection;

        public void Initialize()
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            _connection = factory.CreateConnection();

            DeclareExchange();
            DeclareQueue();
            BindQueueToExchange();

            _initialized = true;
        }

        private void BindQueueToExchange()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueBind(BookRequestQueueName, BookOrderExchangeName, BookRequestQueueName);
            }
        }

        private void DeclareQueue()
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(BookRequestQueueName, durable:false, exclusive: false, autoDelete:true);
            }
        }

        /// <summary>
        /// Attach an instance of the use case from the domain that will handle add book title requests
        /// from the rabbit mq containing book order messages
        /// </summary>
        /// <param name="addBookTitleRequestUseCase"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Handle(AddBookTitleRequestUseCase addBookTitleRequestUseCase)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            _orderBookUseCaseTrigger = new OrderBookUseCaseTrigger(addBookTitleRequestUseCase, _connection);

            _orderBookUseCaseTrigger.Start();
        }

        private void DeclareExchange()
        {            
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(BookOrderExchangeName, ExchangeType.Topic,
                    durable: false, autoDelete: true);
            }                                
        }

        public void Shutdown()
        {            
            _orderBookUseCaseTrigger.Stop();
            _connection.Dispose();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
