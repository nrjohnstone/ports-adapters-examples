using System;

namespace Domain.Events
{
    public class BookOrderLineEditedEvent : IEvent
    {
        public string Title { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public Guid OrderLineId { get; }
        public Guid OrderId { get; }

        public BookOrderLineEditedEvent(string title, decimal price, int quantity, Guid orderLineId, Guid orderId)
        {
            Title = title;
            Price = price;
            Quantity = quantity;
            OrderLineId = orderLineId;
            OrderId = orderId;
        }

        public string GetEventType() => EventType;
        public static string EventType => "BookOrderLineEdited";
    }
}