using System;
using System.Data;
using Dapper;

namespace Adapters.Persistence.MySql.Repositories.Actions
{
    internal class RemoveBookOrderLineAction
    {
        private readonly IDbConnection _connection;

        public RemoveBookOrderLineAction(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Execute(Guid orderLineId)
        {
            _connection.Execute(sql: "delete from book_order_lines " +
                                     "where order_line_id = ?OrderLineId", 
                param: new { OrderLineId = orderLineId});
        }
    }
}