using System;

namespace Domain.Entities
{
    public abstract class BookOrderLineConflict
    {
        public Guid Id { get; }
        public Guid BookOrderId { get; }
        public Guid BookOrderLineId { get; }
        public ConflictType ConflictType { get; }
        public string ConflictValue { get; protected set; }
        public bool Accepted { get; }

        protected BookOrderLineConflict(Guid id, Guid bookOrderId, ConflictType conflictType, Guid bookOrderLineId, bool accepted)
        {
            Id = id;
            BookOrderId = bookOrderId;
            ConflictType = conflictType;
            BookOrderLineId = bookOrderLineId;
            Accepted = accepted;
        }
    }
}