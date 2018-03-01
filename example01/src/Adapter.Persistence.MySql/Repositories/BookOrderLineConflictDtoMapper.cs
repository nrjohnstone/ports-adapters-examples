using System;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories
{
    internal static class BookOrderLineConflictDtoMapper
    {
        internal static BookOrderLineConflictDto From(BookOrderLineConflict bookOrderLineConflict)
        {
            BookOrderLineConflictDto dto = new BookOrderLineConflictDto()
            {
                Id = bookOrderLineConflict.Id,
                conflict_type = bookOrderLineConflict.ConflictType.ToString(),
                Order_Id = bookOrderLineConflict.BookOrderId,
                Order_Line_Id = Guid.NewGuid()
            };
            return dto;
        }
    }
}