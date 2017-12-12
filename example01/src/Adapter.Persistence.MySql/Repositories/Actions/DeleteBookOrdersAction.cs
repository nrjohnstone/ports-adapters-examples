using System.Data;
using Dapper;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal class DeleteBookOrdersAction
    {
        public static void Execute(IDbConnection connection)
        {
            connection.Execute("DELETE FROM book_order_lines");
            connection.Execute("DELETE FROM book_orders");
        }
    }
}