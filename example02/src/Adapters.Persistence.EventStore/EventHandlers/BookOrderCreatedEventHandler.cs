using System.Collections.Generic;
using Domain.Entities;
using Domain.Events;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public class BookOrderCreatedEventHandler : BookOrderEventHandler<BookOrderCreatedEvent>
    {
        public override string EventType => BookOrderCreatedEvent.EventType;

        protected override void DoHandle(BookOrderCreatedEvent ev, BookOrderResult result)
        {
            result.BookOrder = BookOrder.CreateExisting(ev.Supplier, BookOrderState.New, ev.Id,
                new List<OrderLine>());
        }
    }
}