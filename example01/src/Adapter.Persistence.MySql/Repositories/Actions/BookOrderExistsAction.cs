using System.Data;
using Dapper;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal class BookOrderExistsAction
    {
        public static bool Execute(IDbConnection connection, BookOrder bookOrder)
        {
            var parameters = new { orderId = bookOrder.Id };
            var bookOrderExists = connection.QueryFirstOrDefault<bool>("SELECT 1 " +
                                                                       "FROM book_orders WHERE order_id = ?orderId",
                parameters);

            return bookOrderExists;
        }
    }
}