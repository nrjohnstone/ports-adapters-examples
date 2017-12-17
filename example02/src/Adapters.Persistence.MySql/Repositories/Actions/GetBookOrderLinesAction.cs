using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Adapters.Persistence.MySql.Repositories.Dtos;
using Dapper;

namespace Adapters.Persistence.MySql.Repositories.Actions
{
    internal class GetBookOrderLinesAction
    {
        private readonly IDbConnection _connection;

        public GetBookOrderLinesAction(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<BookOrderLineDto> Execute(Guid orderId)
        {
            IEnumerable<BookOrderLineDto> results = _connection.Query<BookOrderLineDto>(
                "select * from book_order_lines where order_id = ?OrderId",
                new { OrderId = orderId});
            return results.ToList();
        }
    }
}