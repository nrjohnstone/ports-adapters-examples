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

        protected BookOrderLineConflict(Guid id, Guid bookOrderId, ConflictType conflictType, Guid bookOrderLineId)
        {
            Id = id;
            BookOrderId = bookOrderId;
            ConflictType = conflictType;
            BookOrderLineId = bookOrderLineId;
        }
    }
}