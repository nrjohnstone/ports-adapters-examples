using System;

namespace Adapter.Persistence.CouchDb.Repositories.Dtos
{
    internal class BookOrderLineConflictDto
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public Guid BookOrderId { get; set; }
        public Guid BookOrderLineId { get; set; }
        public string ConflictValue { get; set; }
        public bool Accepted { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}