using System;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Domain.Entities;

namespace Adapter.Persistence.MySql.Repositories
{
    internal static class BookOrderLineConflictMapper
    {
        internal static BookOrderLineConflict From(BookOrderLineConflictDto dto)
        {
            return BookOrderLineConflict.CreateExisting(dto.Id,
                dto.Order_Id, (ConflictType) Enum.Parse(typeof(ConflictType), dto.conflict_type));
        }

    }
}