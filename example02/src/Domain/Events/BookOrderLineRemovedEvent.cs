using System;

namespace Domain.Events
{
    public class BookOrderLineRemovedEvent : IEvent
    {
        public Guid OrderLineId { get; }
        public Guid OrderId { get; }

        public BookOrderLineRemovedEvent(Guid orderLineId, Guid orderId)
        {
            OrderLineId = orderLineId;
            OrderId = orderId;
        }
        
        public string GetEventType() => EventType;
        public static string EventType => "BookOrderLineMoved";
    }
}