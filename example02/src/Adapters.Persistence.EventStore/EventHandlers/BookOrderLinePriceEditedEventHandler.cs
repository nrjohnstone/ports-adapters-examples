using System.Text;
using Domain.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public class BookOrderLinePriceEditedEventHandler : BookOrderEventHandler<BookOrderLinePriceEditedEvent>
    {
        public override string EventType => BookOrderLinePriceEditedEvent.EventType;

        protected override void DoHandle(BookOrderLinePriceEditedEvent ev, BookOrderResult result)
        {
            result.BookOrder.UpdateOrderLinePrice(ev.OrderLineId, ev.Price);
        }
    }
}