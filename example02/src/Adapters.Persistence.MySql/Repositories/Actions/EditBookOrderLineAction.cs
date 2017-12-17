using System;
using System.Data;
using Dapper;

namespace Adapters.Persistence.MySql.Repositories.Actions
{
    internal class EditBookOrderLineAction
    {
        private readonly IDbConnection _connection;

        public EditBookOrderLineAction(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Execute(Guid orderId, Guid orderLineId, string title, decimal price, int quantity)
        {
            _connection.Execute(
                sql: "update book_order_lines " +
                     "set title = ?Title, " +
                     "price = ?Price, " +
                     "quantity = ?Quantity " +
                     "where order_line_id = ?OrderLineId", param: new
                {
                    OrderLineId = orderLineId,
                    Title = title,
                    Price = price,
                    Quantity =quantity
                });
        }
    }
}