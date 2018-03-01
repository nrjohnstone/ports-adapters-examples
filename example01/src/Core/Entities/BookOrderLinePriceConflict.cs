using System;
using System.Globalization;

namespace Domain.Entities
{
    public class BookOrderLinePriceConflict : BookOrderLineConflict
    {
        public decimal Price { get; }

        protected BookOrderLinePriceConflict(Guid id, Guid bookOrderId, ConflictType conflictType, Guid bookOrderLineId,
            decimal price, bool accepted)
            : base(id, bookOrderId, conflictType, bookOrderLineId, accepted)
        {
            Price = price;
            ConflictValue = price.ToString(CultureInfo.InvariantCulture);
        }

        public static BookOrderLinePriceConflict CreateNew(Guid bookOrderId, Guid bookOrderLineId, decimal price)
        {

            return new BookOrderLinePriceConflict(
                Guid.NewGuid(), bookOrderId, ConflictType.Price, bookOrderLineId, price, false);
        }

        public static BookOrderLinePriceConflict CreateExisting(Guid id, Guid bookOrderId,
            Guid bookOrderLineId, decimal price, bool accepted)
        {
            return new BookOrderLinePriceConflict(
                id, bookOrderId, ConflictType.Price, bookOrderLineId, price, accepted);
        }
    }
}