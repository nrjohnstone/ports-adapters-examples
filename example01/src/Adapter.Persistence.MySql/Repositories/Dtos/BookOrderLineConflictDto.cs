using System;

namespace Adapter.Persistence.MySql.Repositories.Dtos
{
    internal class BookOrderLineConflictDto
    {
        public Guid Id { get; set; }
        public Guid Order_Id { get; set; }
        public Guid Order_Line_Id { get; set; }
        public string conflict_type { get; set; }
        public string Conflict_Value { get; set; }
        public bool Accepted { get; set; }
        public DateTime Created_DateTime { get; set; }
    }
}