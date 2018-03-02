using System.Data;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Dapper;

namespace Adapter.Persistence.MySql.Repositories.Actions
{
    internal class InsertBookOrderLineConflictAction
    {
        public static void Execute(IDbConnection connection, BookOrderLineConflictDto bookOrderLineConflictDto)
        {
            connection.Execute("INSERT INTO book_order_line_conflicts (" +
                               "id, " +
                               "order_id, " +
                               "order_line_id, " +
                               "conflict_type," +
                               "conflict_value, " +
                               "accepted," +
                               "created_datetime) " +
                               "VALUES (?id, ?orderId, ?orderLineId, ?conflictType," +
                               "?conflictValue," +
                               "?accepted," +
                               "?createdDateTime)",
                new
                {
                    id = bookOrderLineConflictDto.Id,
                    orderId = bookOrderLineConflictDto.Order_Id,
                    orderLineId = bookOrderLineConflictDto.Order_Line_Id,
                    conflictType = bookOrderLineConflictDto.conflict_type,
                    conflictValue = bookOrderLineConflictDto.Conflict_Value,
                    accepted = bookOrderLineConflictDto.Accepted,
                    createdDateTime = bookOrderLineConflictDto.Created_DateTime
                });
        }
    }
}