using System.Collections.Generic;

namespace HostApp.WebService.Client3.BookOrders
{
    public class BookOrderResponse
    {
        public string Supplier { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public IEnumerable<OrderLineResponse> OrderLines { get; set; }
    }
}