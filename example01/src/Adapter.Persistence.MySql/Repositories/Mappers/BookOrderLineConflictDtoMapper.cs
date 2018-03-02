using Adapter.Persistence.MySql.Repositories.Dtos;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories.Mappers
{
    internal static class BookOrderLineConflictDtoMapper
    {
        internal static BookOrderLineConflictDto ToDto(this BookOrderLineConflict bookOrderLineConflict)
        {
            BookOrderLineConflictDto dto = new BookOrderLineConflictDto()
            {
                Id = bookOrderLineConflict.Id,
                conflict_type = bookOrderLineConflict.ConflictType.ToString(),
                Order_Id = bookOrderLineConflict.BookOrderId,
                Order_Line_Id = bookOrderLineConflict.BookOrderLineId,
                Conflict_Value = bookOrderLineConflict.ConflictValue,
                Accepted = bookOrderLineConflict.Accepted,
                Created_DateTime = bookOrderLineConflict.CreatedDateTime
            };
            return dto;
        }
    }
}