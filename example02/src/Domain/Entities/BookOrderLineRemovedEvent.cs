using System;
using Domain.Events;

namespace Domain.Entities
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
    }
}