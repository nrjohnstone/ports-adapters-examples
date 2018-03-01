using System.Collections.Generic;
using System.Data;
using Adapter.Persistence.MySql.Repositories.Dtos;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal static class GetBookOrderLineConflictsAction
    {
        public static IEnumerable<BookOrderLineConflictDto> Execute(
            IDbConnection connection)
        {
            IEnumerable<BookOrderLineConflictDto> results =
                new List<BookOrderLineConflictDto>();
            return results;
        }
    }
}