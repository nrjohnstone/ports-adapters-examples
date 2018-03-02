using System;
using System.Collections.Generic;

namespace Host.WebService.Client1.BookOrders
{
    public class SupplierBookOrderUpdateDto
    {
        public IEnumerable<SupplierBookOrderLineUpdateDto> OrderLineUpdates { get; set; }
    }
}