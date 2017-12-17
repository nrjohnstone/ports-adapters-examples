using System;
using System.Data;
using Adapters.Persistence.MySql.Repositories.Dtos;
using Dapper;

namespace Adapters.Persistence.MySql.Repositories.Actions
{
    internal class GetBookOrderAction
    {
        private readonly IDbConnection _connection;

        public GetBookOrderAction(IDbConnection connection)
        {
            _connection = connection;
        }

        public BookOrderDto Execute(Guid orderId)
        {
            var result = _connection.QuerySingle<BookOrderDto>(
                "select order_id, supplier, state from book_orders " +
                "where order_id = ?OrderId",
                new { OrderId = orderId});
            return result;
        }
    }
}