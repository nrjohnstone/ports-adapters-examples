using System.Collections.Generic;

namespace HostApp.WebService.Client1.Dtos
{
    public class SupplierBookOrderUpdateDto
    {
        public IEnumerable<SupplierBookOrderLineUpdateDto> OrderLineUpdates { get; set; }
    }
}