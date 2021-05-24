using System.Collections.Generic;

namespace HostApp.WebService.Client2.Tests.Acceptance.Dtos
{
    internal class BookOrderResponseDto
    {
        public string Supplier { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public IEnumerable<OrderLineResponseDto> OrderLines { get; set; }
    }
}