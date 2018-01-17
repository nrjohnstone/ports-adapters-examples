using System;
using System.Data;
using Dapper;

namespace Adapters.Persistence.MySql.Repositories.Actions
{
    internal class EditBookOrderLinePriceAction
    {
        private readonly IDbConnection _connection;

        public EditBookOrderLinePriceAction(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Execute(Guid orderId, Guid orderLineId, decimal price)
        {
            _connection.Execute(
                sql: "update book_order_lines " +
                     "set price = ?Price " +
                     "where order_line_id = ?OrderLineId" , param: new
                {
                    OrderLineId = orderLineId,
                    Price = price
                });
        }
    }
}