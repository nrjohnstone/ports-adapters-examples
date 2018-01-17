using System.Text;
using Domain.Entities;
using Domain.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public class BookOrderLineCreatedEventHandler : BookOrderEventHandler<BookOrderLineCreatedEvent>
    {
        public override string EventType => BookOrderLineCreatedEvent.EventType;

        protected override void DoHandle(BookOrderLineCreatedEvent ev, BookOrderResult result)
        {
            OrderLine ol = new OrderLine(ev.Title, ev.Price, 
                ev.Quantity, ev.OrderLineId);
            result.BookOrder.CreateExistingOrderLine(ol);
        }
    }
}