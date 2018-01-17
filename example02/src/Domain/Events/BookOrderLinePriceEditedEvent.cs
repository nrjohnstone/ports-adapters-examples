using System;

namespace Domain.Events
{
    public class BookOrderLinePriceEditedEvent : IEvent
    {
        public decimal Price { get; }
        public Guid OrderLineId { get; }
        public Guid OrderId { get; }

        public BookOrderLinePriceEditedEvent(Guid orderId, Guid orderLineId, decimal price)
        {
            Price = price;
            OrderLineId = orderLineId;
            OrderId = orderId;
        }

        public string GetEventType() => EventType;
        public static string EventType => "BookOrderLinePriceEdited";
    }
}