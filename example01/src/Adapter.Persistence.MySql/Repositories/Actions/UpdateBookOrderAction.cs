using System.Data;
using Dapper;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal class UpdateBookOrderAction
    {
        public static void Execute(IDbConnection connection, BookOrder bookOrder)
        {
            connection.Execute("UPDATE book_orders " +
                               "SET supplier = ?supplier, " +
                               "state = ?state " +
                               "WHERE order_id = ?orderId",
                new
                {
                    orderId = bookOrder.Id,
                    supplier = bookOrder.Supplier,
                    state = bookOrder.State
                });

            connection.Execute("DELETE FROM book_order_lines " +
                               "WHERE order_id = ?orderId",
                new {orderId = bookOrder.Id});


            foreach (var bookOrderOrderLine in bookOrder.OrderLines)
            {
                connection.Execute("INSERT INTO book_order_lines (order_line_id, order_id, title, price, quantity) " +
                                   "VALUES (?orderLineId, ?orderId, ?title, ?price, ?quantity)",
                    new
                    {
                        orderLineId = bookOrderOrderLine.Id,
                        orderId = bookOrder.Id,
                        title = bookOrderOrderLine.Title,
                        price = bookOrderOrderLine.Price,
                        quantity = bookOrderOrderLine.Quantity
                    });
            }
        }
    }
}