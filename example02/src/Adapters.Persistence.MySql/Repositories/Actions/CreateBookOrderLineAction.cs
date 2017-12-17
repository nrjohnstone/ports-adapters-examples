using System;
using System.Data;
using Dapper;

namespace Adapters.Persistence.MySql.Repositories.Actions
{
    internal class CreateBookOrderLineAction
    {
        private readonly IDbConnection _connection;

        public CreateBookOrderLineAction(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Execute(Guid orderId, Guid orderLineId, string title, decimal price, int quantity)
        {
            var parameters = new DynamicParameters();

            parameters.Add("OrderId", orderId);
            parameters.Add("OrderLineId", orderLineId);
            parameters.Add("Title", title);
            parameters.Add("Price", price);
            parameters.Add("Quantity", quantity);

            _connection.Execute(sql: "insert into book_order_lines (" +
                                     "order_line_id, " +
                                     "order_id, " +
                                     "title, " +
                                     "price, " +
                                     "quantity )" +
                                     " VALUES " +
                                     "( ?OrderLineId, " +
                                     "?OrderId, " +
                                     "?Title, " +
                                     "?Price, " +
                                     "?Quantity )",
                param: parameters);
        }
    }
}