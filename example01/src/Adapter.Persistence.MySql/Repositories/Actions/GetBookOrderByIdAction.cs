using System;
using System.Collections.Generic;
using System.Data;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Dapper;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal class GetBookOrderByIdAction
    {
        public static BookOrder Execute(IDbConnection connection, Guid id)
        {
            var bookOrderDto = connection.QueryFirstOrDefault<BookOrderDto>(
                "SELECT supplier, order_id, state FROM book_orders WHERE order_id = ?orderId",
                new { orderId = id });

            if (bookOrderDto == null)
                return null;

            var orderLineDtos = connection.Query<OrderLineDto>(
                "SELECT order_line_id, " +
                "order_id," +
                "price, quantity, title FROM book_order_lines WHERE order_id = ?orderId",
                new { orderId = id });

            List<OrderLine> orderLines = new List<OrderLine>();

            foreach (var orderLineDto in orderLineDtos)
            {
                orderLines.Add(new OrderLine(
                    orderLineDto.Title,
                    orderLineDto.Price,
                    orderLineDto.Quantity,
                    orderLineDto.Order_Line_Id));
            }

            return BookOrder.CreateExisting(bookOrderDto.Supplier,
                bookOrderDto.Order_Id,
                bookOrderDto.State,
                orderLines);
        }
    }
}