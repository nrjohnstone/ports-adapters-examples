using System;

namespace Adapter.Persistence.MySql.Repositories.Dtos
{
    internal class BookOrderLineConflictDto
    {
        public Guid Id { get; set; }
        public Guid Order_Id { get; set; }
        public Guid Order_Line_Id { get; set; }
        public string ConflictType { get; set; }
    }
}