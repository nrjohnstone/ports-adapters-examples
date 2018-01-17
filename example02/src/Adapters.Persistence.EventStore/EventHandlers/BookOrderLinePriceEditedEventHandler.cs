using System.Text;
using Domain.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public class BookOrderLinePriceEditedEventHandler : IBookOrderEventHandler
    {
        public bool CanHandle(RecordedEvent ev)
        {
            if (ev.EventType.Equals(BookOrderLinePriceEditedEvent.EventType))
                return true;

            return false;
        }

        public void Handle(RecordedEvent recordedEvent, BookOrderResult result)
        {
            var st = Encoding.ASCII.GetString(recordedEvent.Data);
            BookOrderLinePriceEditedEvent ev =
                JsonConvert.DeserializeObject<BookOrderLinePriceEditedEvent>(st);

            result.BookOrder.UpdateOrderLinePrice(ev.OrderLineId, ev.Price);
        }
    }
}