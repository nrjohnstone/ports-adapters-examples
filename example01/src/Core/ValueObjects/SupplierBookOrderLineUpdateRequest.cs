using System;

namespace Domain.ValueObjects
{
    public class SupplierBookOrderLineUpdateRequest
    {
        public SupplierBookOrderLineUpdateRequest(Guid bookOrderLineId,
            decimal price,
            int quantity)
        {
            BookOrderLineId = bookOrderLineId;
            Price = price;
            Quantity = quantity;
        }

        public int Quantity { get; }
        public Guid BookOrderLineId { get; }
        public decimal Price { get; }
    }
}