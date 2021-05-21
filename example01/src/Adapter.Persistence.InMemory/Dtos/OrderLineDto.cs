using System;

namespace Adapter.Persistence.InMemory.Dtos
{
    internal class OrderLineDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}