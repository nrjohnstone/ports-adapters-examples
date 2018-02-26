using System;
using System.Collections.Generic;
using System.Data;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Dapper;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal class GetBookOrdersAction
    {
        public static IEnumerable<BookOrder> Execute(IDbConnection connection, string supplier = null,
            BookOrderState? state = null)
        {
            DynamicParameters parameters = new DynamicParameters();
            var sqlStatement =
                "SELECT supplier, order_id, state FROM book_orders ";

            var sqlConditional = "";

            if (supplier != null)
            {
                sqlConditional = EvaluateSqlConditional(sqlConditional);
                sqlConditional += " supplier = ?supplier";
                parameters.Add("supplier", supplier);
            }

            if (state.HasValue)
            {
                sqlConditional = EvaluateSqlConditional(sqlConditional);
                sqlConditional += " state = ?state";
                parameters.Add("state", state);
            }

            sqlStatement = sqlStatement + sqlConditional;

            var bookOrderDtos = connection.Query<BookOrderDto>(sqlStatement, parameters);

            List<BookOrder> bookOrders = new List<BookOrder>();

            foreach (var bookOrderDto in bookOrderDtos)
            {
                List<OrderLine> orderLines = new List<OrderLine>();
                var orderLineDtos = connection.Query<OrderLineDto>(
                    "SELECT order_line_id, " +
                    "order_id," +
                    "price, quantity, title FROM book_order_lines WHERE order_id = ?orderId",
                    new { orderId = bookOrderDto.Order_Id });
                foreach (var orderLineDto in orderLineDtos)
                {
                    orderLines.Add(new OrderLine(
                        orderLineDto.Title,
                        orderLineDto.Price,
                        orderLineDto.Quantity,
                        orderLineDto.Order_Line_Id));
                }

                bookOrders.Add(BookOrder.CreateExisting(
                    bookOrderDto.Supplier,
                    bookOrderDto.Order_Id,
                    bookOrderDto.State,
                    orderLines));
            }

            return bookOrders;
        }

        private static string EvaluateSqlConditional(string sqlConditional)
        {
            if (String.IsNullOrEmpty(sqlConditional))
                sqlConditional += " WHERE ";
            else
                sqlConditional += " AND ";
            return sqlConditional;
        }
    }
}