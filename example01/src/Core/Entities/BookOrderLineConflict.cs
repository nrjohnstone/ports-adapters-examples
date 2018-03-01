using System;

namespace Domain.Entities
{
    public class BookOrderLineConflict
    {
        public Guid Id { get; }
        public Guid BookOrderId { get; }
        public Guid BookOrderLineId { get; }
        public ConflictType ConflictType { get; }

        private BookOrderLineConflict(Guid id, Guid bookOrderId, ConflictType conflictType, Guid bookOrderLineId)
        {
            Id = id;
            BookOrderId = bookOrderId;
            ConflictType = conflictType;
            BookOrderLineId = bookOrderLineId;
        }

        public static BookOrderLineConflict CreateNew(Guid bookOrderId, ConflictType conflictType, Guid bookOrderLineId)
        {
            return new BookOrderLineConflict(Guid.NewGuid(), bookOrderId, conflictType, bookOrderLineId);
        }

        public static BookOrderLineConflict CreateExisting(Guid id, Guid bookOrderId, ConflictType conflictType,
            Guid bookOrderLineId)
        {
            return new BookOrderLineConflict(id, bookOrderId, conflictType, bookOrderLineId);
        }
    }
}