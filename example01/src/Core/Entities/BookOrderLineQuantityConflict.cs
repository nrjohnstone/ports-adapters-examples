using System;
using System.Globalization;

namespace Domain.Entities
{
    public class BookOrderLineQuantityConflict : BookOrderLineConflict
    {
        public int Quantity { get; }

        protected BookOrderLineQuantityConflict(Guid id, Guid bookOrderId, ConflictType conflictType,
            Guid bookOrderLineId, int quantity, bool accepted) : base(id, bookOrderId, conflictType, bookOrderLineId, accepted)
        {
            Quantity = quantity;
            ConflictValue = quantity.ToString(CultureInfo.InvariantCulture);
        }

        public static BookOrderLineQuantityConflict CreateNew(Guid bookOrderId, Guid bookOrderLineId, int quantity)
        {
            return new BookOrderLineQuantityConflict(
                Guid.NewGuid(), bookOrderId, ConflictType.Quantity, bookOrderLineId, quantity, false);
        }

        public static BookOrderLineQuantityConflict CreateExisting(Guid id, Guid bookOrderId,
            Guid bookOrderLineId, int quantity, bool accepted)
        {
            return new BookOrderLineQuantityConflict(
                id, bookOrderId, ConflictType.Quantity, bookOrderLineId, quantity, accepted);
        }
    }
}