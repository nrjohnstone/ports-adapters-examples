using Domain.Events;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public class BookOrderLineRemovedEventHandler : BookOrderEventHandler<BookOrderLineRemovedEvent>
    {
        public override string EventType => BookOrderLineRemovedEvent.EventType;

        protected override void DoHandle(BookOrderLineRemovedEvent ev, BookOrderResult result)
        {
            result.BookOrder.RemoveOrderLine(ev.OrderLineId);
        }
    }
}