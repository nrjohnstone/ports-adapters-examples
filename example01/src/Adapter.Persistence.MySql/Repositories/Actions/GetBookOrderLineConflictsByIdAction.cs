using System;
using System.Data;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Dapper;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal static class GetBookOrderLineConflictsByIdAction
    {
        public static BookOrderLineConflictDto Execute(IDbConnection connection ,Guid id)
        {
            var result = connection.QuerySingleOrDefault<BookOrderLineConflictDto>(
                "SELECT " +
                "id, " +
                "order_id, " +
                "order_line_id, " +
                "conflict_type, " +
                "conflict_value, " +
                "accepted," +
                "created_datetime " +
                "FROM book_order_line_conflicts " +
                "WHERE id = ?id",
                new { id = id });
            return result;
        }
    }
}