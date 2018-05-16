using System;
using System.Globalization;

namespace Domain.Entities
{
    public class BookOrderLinePriceConflict : BookOrderLineConflict
    {
        public decimal Price { get; }

        protected BookOrderLinePriceConflict(Guid id, Guid bookOrderId, ConflictType conflictType, Guid bookOrderLineId,
            decimal price, bool accepted, DateTime createdDateTime)
            : base(id, bookOrderId, conflictType, bookOrderLineId, accepted, createdDateTime)
        {
            Price = price;
            ConflictValue = price.ToString(CultureInfo.InvariantCulture);
        }

        public static BookOrderLinePriceConflict CreateNew(Guid bookOrderId, Guid bookOrderLineId, decimal price)
        {
            DateTime currentDateTime = DateTime.UtcNow;
            return new BookOrderLinePriceConflict(
                Guid.NewGuid(), bookOrderId, ConflictType.Price, bookOrderLineId, price, false, currentDateTime);
        }

        public static BookOrderLinePriceConflict CreateExisting(Guid id, Guid bookOrderId,
            Guid bookOrderLineId, decimal price, bool accepted, DateTime createdDateTime)
        {
            return new BookOrderLinePriceConflict(
                id, bookOrderId, ConflictType.Price, bookOrderLineId, price, accepted, createdDateTime);
        }
    }
}