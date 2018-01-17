using System;
using System.Collections.Generic;
using System.Data;
using Adapters.Persistence.MySql.Repositories.Actions;
using Domain.Entities;
using Domain.Events;
using Domain.Ports.Persistence;
using Microsoft.CSharp.RuntimeBinder;
using MySql.Data.MySqlClient;

namespace Adapters.Persistence.MySql.Repositories
{
    public class BookOrderRepository : IBookOrderRepository
    {
        private readonly string _connectionString;

        public BookOrderRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public void Store(BookOrder bookOrder)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var events = bookOrder.DequeueAllEvents();

                    foreach (var ev in events)
                    {
                        HandleEvent(ev, connection);
                    }
                  
                    transaction.Commit();
                }
            }
        }

        // NOTE: In a production system you want to let the domain be in charge of the transaction
        // scope so you would not open and close a connect for each call to a repository
        // but in the interests of keeping things simple we do it this way here
        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private void HandleEvent(IEvent ev, IDbConnection connection)
        {
            try
            {
                InvokeHandlerUnsafe(ev, connection);
            }
            catch (RuntimeBinderException)
            {
                throw 
                    new MissingMethodException("No event handler " +
                                               $"found for event {ev.GetType()}.");
            }
        }

        private void InvokeHandlerUnsafe(IEvent ev, IDbConnection connection)
        {
            this.Handle((dynamic)ev, connection);
        }

        private void Handle(BookOrderCreatedEvent ev, IDbConnection connection)
        {
            var action = new CreateBookOrderAction(connection);
            action.Execute(ev.Id, ev.Supplier);
        }

        private void Handle(BookOrderLineCreatedEvent ev, IDbConnection connection)
        {
            var action = new CreateBookOrderLineAction(connection);
            action.Execute(ev.OrderId, ev.OrderLineId, ev.Title, ev.Price, ev.Quantity);
        }

        private void Handle(BookOrderLinePriceEditedEvent ev, IDbConnection connection)
        {
            var action = new EditBookOrderLinePriceAction(connection);
            action.Execute(ev.OrderId, ev.OrderLineId, ev.Price);
        }

        private void Handle(BookOrderLineRemovedEvent ev, IDbConnection connection)
        {
            var action = new RemoveBookOrderLineAction(connection);
            action.Execute(ev.OrderLineId);
        }

        public BookOrder Get(Guid orderId)
        {
            using (var connection = CreateConnection())
            {
                var bookOrderDto = new GetBookOrderAction(connection).Execute(orderId);
                var bookOrderLineDtos = new GetBookOrderLinesAction(connection).Execute(bookOrderDto.Order_Id);

                List<OrderLine> lines = OrderLineFactory.CreateFrom(bookOrderLineDtos);
                
                var bookOrder = BookOrder.CreateExisting(
                    bookOrderDto.Supplier, bookOrderDto.State, bookOrderDto.Order_Id, lines);

                return bookOrder;
            }            
        }
    }
}
