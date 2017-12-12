using System.Data;
using Dapper;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal class InsertBookOrderAction
    {
        public static void Execute(IDbConnection connection, BookOrder bookOrder)
        {
            connection.Execute("INSERT INTO book_orders (order_id,supplier,state) VALUES (?orderId, ?supplier, ?state)",
                new
                {
                    orderId = bookOrder.Id,
                    supplier = bookOrder.Supplier,
                    state = bookOrder.State
                });

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