using System.Collections.Generic;
using System.Data;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Dapper;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal static class GetBookOrderLineConflictsAction
    {
        public static IEnumerable<BookOrderLineConflictDto> Execute(
            IDbConnection connection)
        {
            var results = connection.Query<BookOrderLineConflictDto>(
                "SELECT " +
                "id, " +
                "order_id, " +
                "order_line_id, " +
                "conflict_type, " +
                "conflict_value, " +
                "accepted " +
                "FROM book_order_line_conflicts ");
            return results;
        }
    }
}