using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public class BookOrderCreatedEventHandler : IBookOrderEventHandler
    {
        public bool CanHandle(RecordedEvent ev)
        {
            if (ev.EventType.Equals(BookOrderCreatedEvent.EventType))
                return true;

            return false;
        }

        public void Handle(RecordedEvent recordedEvent, BookOrderResult result)
        {
            var st = Encoding.ASCII.GetString(recordedEvent.Data);
            BookOrderCreatedEvent ev = JsonConvert.DeserializeObject<BookOrderCreatedEvent>(st);
            result.BookOrder = BookOrder.CreateExisting(ev.Supplier, BookOrderState.New, ev.Id,
                new List<OrderLine>());
        }
    }
}