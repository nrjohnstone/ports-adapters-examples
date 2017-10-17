using System;
using System.Text;
using AmbientContext.LogService.Serilog;
using Core.UseCases;
using Core.ValueObjects;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static Adapter.Trigger.RabbitMq.RabbitMqConstants;

namespace Adapter.Trigger.RabbitMq
{
    internal class OrderBookUseCaseTrigger
    {
        public AmbientLogService Logger { get; } = new AmbientLogService();
        private readonly OrderBookUseCase _orderBookUseCase;
        private readonly IConnection _connection;
        private IModel _channel;

        public OrderBookUseCaseTrigger(OrderBookUseCase orderBookUseCase, IConnection connection)
        {
            _orderBookUseCase = orderBookUseCase;
            _connection = connection;
        }

        public void Start()
        {
            _channel = _connection.CreateModel();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) => OnReceive(args);

            _channel.BasicConsume(BookRequestQueueName, false, consumer);                                                
        }

        private void OnReceive(BasicDeliverEventArgs args)
        {
            var message = Encoding.UTF8.GetString(args.Body);
            Logger.Debug("Received {RabbitMqMessage}", message);

            try
            {
                var bookRequest = JsonConvert.DeserializeObject<BookTitleOrder>(message);
                _orderBookUseCase.Execute(bookRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.StackTrace}");
            }
        }

        public void Stop()
        {
            _channel.Close();
        }        
    }
}