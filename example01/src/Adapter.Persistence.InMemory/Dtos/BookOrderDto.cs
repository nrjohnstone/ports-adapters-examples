using System;
using System.Collections.Generic;

namespace Adapter.Persistence.InMemory.Dtos
{
    internal class BookOrderDto
    {
        public string Supplier { get; set; }
        public Guid Id { get; set; }
        public List<OrderLineDto> OrderLines { get; set; }
        public BookOrderStateDto State { get; set; }
    }
}