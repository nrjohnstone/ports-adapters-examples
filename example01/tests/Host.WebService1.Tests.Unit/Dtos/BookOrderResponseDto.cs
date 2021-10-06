using System.Collections.Generic;

namespace HostApp.WebService.Client1.Tests.Unit.Dtos
{
    internal class BookOrderResponseDto
    {
        public string Supplier { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public IList<OrderLineResponseDto> OrderLines { get; set; }
    }
}