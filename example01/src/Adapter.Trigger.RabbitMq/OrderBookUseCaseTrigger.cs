using System;
using System.Text;
using AmbientContext.LogService.Serilog;
using Domain.UseCases;
using Domain.ValueObjects;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static Adapter.Trigger.RabbitMq.RabbitMqConstants;

namespace Adapter.Trigger.RabbitMq
{
    internal class OrderBookUseCaseTrigger
    {
        public AmbientLogService Logger { get; } = new AmbientLogService();
        private readonly AddBookTitleRequestUseCase _addBookTitleRequestUseCase;
        private readonly IConnection _connection;
        private IModel _channel;

        public OrderBookUseCaseTrigger(AddBookTitleRequestUseCase addBookTitleRequestUseCase, IConnection connection)
        {
            _addBookTitleRequestUseCase = addBookTitleRequestUseCase;
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
                var bookRequest = JsonConvert.DeserializeObject<BookTitleRequest>(message);
                _addBookTitleRequestUseCase.Execute(bookRequest);
                _channel.BasicAck(args.DeliveryTag, false);
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