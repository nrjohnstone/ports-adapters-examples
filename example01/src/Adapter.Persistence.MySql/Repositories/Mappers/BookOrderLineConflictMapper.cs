using System;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories.Mappers
{
    internal static class BookOrderLineConflictMapper
    {
        internal static BookOrderLineConflict ToEntity(this BookOrderLineConflictDto dto)
        {
            if (dto.conflict_type.Equals("Quantity"))
            {
                int quantity = Convert.ToInt32(dto.Conflict_Value);
                return BookOrderLineQuantityConflict.CreateExisting(dto.Id,
                    dto.Order_Id,
                    dto.Order_Line_Id, quantity, dto.Accepted,
                    dto.Created_DateTime);
            }
            else if (dto.conflict_type.Equals("Price"))
            {
                decimal price = Convert.ToDecimal(dto.Conflict_Value);
                return BookOrderLinePriceConflict.CreateExisting(
                    dto.Id, dto.Order_Id, dto.Order_Line_Id, price, dto.Accepted,
                    dto.Created_DateTime);
            }

            throw new ArgumentOutOfRangeException();

        }
    }
}