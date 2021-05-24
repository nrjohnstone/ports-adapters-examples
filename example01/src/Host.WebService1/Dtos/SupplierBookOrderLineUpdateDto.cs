using System;

namespace HostApp.WebService.Client1.Dtos
{
    public class SupplierBookOrderLineUpdateDto
    {
        public Guid BookOrderLineId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}