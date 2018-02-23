using System;

namespace Domain.Entities
{
    public class BookOrderLineConflict
    {
        public Guid Id { get; }
        public Guid BookOrderId { get; }
        public ConflictType ConflictType { get; }

        private BookOrderLineConflict(Guid id, Guid bookOrderId, ConflictType conflictType)
        {
            Id = id;
            BookOrderId = bookOrderId;
            ConflictType = conflictType;
        }

        public static BookOrderLineConflict CreateNew(Guid bookOrderId, ConflictType conflictType)
        {
            return new BookOrderLineConflict(Guid.NewGuid(), bookOrderId, conflictType);
        }

        public static BookOrderLineConflict CreateExisting(Guid id, Guid bookOrderId, ConflictType conflictType)
        {
            return new BookOrderLineConflict(id, bookOrderId, conflictType);
        }
    }
}