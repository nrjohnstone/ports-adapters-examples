using System.Text;
using Domain.Entities;
using Domain.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public class BookOrderLineCreatedEventHandler : IBookOrderEventHandler
    {
        public bool CanHandle(RecordedEvent ev)
        {
            if (ev.EventType.Equals(BookOrderLineCreatedEvent.EventType))
                return true;

            return false;
        }

        public void Handle(RecordedEvent recordedEvent, BookOrderResult result)
        {
            var st = Encoding.ASCII.GetString(recordedEvent.Data);
            BookOrderLineCreatedEvent ev =
                JsonConvert.DeserializeObject<BookOrderLineCreatedEvent>(st);

            OrderLine ol = new OrderLine(
                ev.Title, ev.Price, ev.Quantity, ev.OrderLineId);
            result.BookOrder.CreateExistingOrderLine(ol);
        }
    }
}