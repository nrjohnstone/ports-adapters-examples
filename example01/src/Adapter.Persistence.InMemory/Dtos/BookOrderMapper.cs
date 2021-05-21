using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Adapter.Persistence.InMemory.Dtos
{        
    /// <summary>
    /// Mapper for translating Domain types to internal DTOs and vice versa
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    internal static class BookOrderMapper
    {
        public static BookOrder ToDomain(this BookOrderDto dto)
        {
            BookOrderState state = dto.State.ToDomain();
            List<OrderLine> orderLines = new List<OrderLine>();
            foreach (var dtoOrderLine in dto.OrderLines)
            {
                orderLines.Add(new OrderLine(dtoOrderLine.Title, dtoOrderLine.Price, dtoOrderLine.Quantity, dtoOrderLine.Id));
            }
            return BookOrder.CreateExisting(dto.Supplier, dto.Id, state, orderLines);
        }
        
        public static IEnumerable<BookOrder> ToDomain(this IEnumerable<BookOrderDto> dtos)
        {
            List<BookOrder> bookOrders = new List<BookOrder>();

            foreach (var dto in dtos)
            {
                BookOrderState state = dto.State.ToDomain();
                List<OrderLine> orderLines = new List<OrderLine>();
                foreach (var dtoOrderLine in dto.OrderLines)
                {
                    orderLines.Add(new OrderLine(dtoOrderLine.Title, dtoOrderLine.Price, dtoOrderLine.Quantity, dtoOrderLine.Id));
                }

                bookOrders.Add(
                    BookOrder.CreateExisting(dto.Supplier, dto.Id, state, orderLines));
            }

            return bookOrders;
        }

        public static BookOrderState ToDomain(this BookOrderStateDto dto)
        {
            return (BookOrderState) Enum.Parse(typeof(BookOrderState), dto.ToString());
        }
        
        public static BookOrderDto ToDto(this BookOrder bookOrder)
        {
            List<OrderLineDto> orderLineDtos = new List<OrderLineDto>();
            foreach (var orderLine in bookOrder.OrderLines)
            {
                orderLineDtos.Add(new OrderLineDto()
                {
                    Id = orderLine.Id,
                    Price = orderLine.Price,
                    Quantity = orderLine.Quantity,
                    Title = orderLine.Title
                });
            }
            
            return new BookOrderDto()
            {
                Id = bookOrder.Id,
                State = bookOrder.State.ToDto(),
                Supplier = bookOrder.Supplier,
                OrderLines = orderLineDtos
            };
        }

        public static BookOrderStateDto ToDto(this BookOrderState bookOrderState)
        {
            return (BookOrderStateDto) Enum.Parse(typeof(BookOrderState), bookOrderState.ToString());
        }
    }
}