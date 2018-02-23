using System;

namespace Domain.ValueObjects
{
    public class SupplierBookOrderLineUpdateRequest
    {
        public SupplierBookOrderLineUpdateRequest(Guid bookOrderLineId, decimal price)
        {
            BookOrderLineId = bookOrderLineId;
            Price = price;
        }

        public Guid BookOrderLineId { get; }
        public decimal Price { get; }
    }
}