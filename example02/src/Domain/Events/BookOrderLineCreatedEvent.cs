using System;

namespace Domain.Events
{
    public class BookOrderLineCreatedEvent : IEvent
    {
        public string Title { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public Guid OrderLineId { get; }
        public Guid OrderId { get; }

        public BookOrderLineCreatedEvent(string title, decimal price, int quantity, Guid orderLineId, Guid orderId)
        {
            Title = title;
            Price = price;
            Quantity = quantity;
            OrderLineId = orderLineId;
            OrderId = orderId;
        }

        public string GetEventType() => EventType;
        public static string EventType => "BookOrderLineCreated";
    }
}