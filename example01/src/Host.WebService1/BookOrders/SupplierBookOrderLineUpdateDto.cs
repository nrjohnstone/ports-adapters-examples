using System;

namespace Host.WebService.Client1.BookOrders
{
    public class SupplierBookOrderLineUpdateDto
    {
        public Guid BookOrderLineId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}