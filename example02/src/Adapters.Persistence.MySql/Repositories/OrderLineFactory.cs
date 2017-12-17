using System.Collections.Generic;
using System.Linq;
using Adapters.Persistence.MySql.Repositories.Dtos;
using Domain.Entities;

namespace Adapters.Persistence.MySql.Repositories
{
    internal static class OrderLineFactory
    {
        public static List<OrderLine> CreateFrom(List<BookOrderLineDto> bookOrderLineDtos)
        {
            return bookOrderLineDtos.Select(
                dto => new OrderLine(dto.Title, dto.Price, dto.Quantity, dto.Order_Line_Id)).ToList();
        }
    }
}