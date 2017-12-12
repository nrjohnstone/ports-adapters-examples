using System;

namespace Adapter.Persistence.CouchDb.Repositories.Dtos
{
    internal class OrderLineDto
    {
        public OrderLineDto(Guid id, string title, decimal price, int quantity, Guid orderId)
        {
            Id = id;
            Title = title;
            Price = price;
            Quantity = quantity;
            OrderId = orderId;
        }

        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}