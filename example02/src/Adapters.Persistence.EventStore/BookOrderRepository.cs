using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Domain.Entities;
using Domain.Ports.Persistence;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Adapters.Persistence.EventStore
{
    public class BookOrderRepository : IBookOrderRepository
    {
        private const string StreamBaseName = "bookOrders";

        public void Store(BookOrder bookOrder)
        {
            using (var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113)))
            {
                connection.ConnectAsync().Wait();

                var events = bookOrder.DequeueAllEvents();

                foreach (var ev in events)
                {
                    string eventType = ev.GetEventType();
                    var eventData = GetEventDataFor(ev, eventType);

                    connection.AppendToStreamAsync($"{StreamBaseName}-{bookOrder.Id}",
                        ExpectedVersion.Any, eventData).Wait();
                }
            }
        }

        public BookOrder Get(Guid orderId)
        {
            BookOrderFactory factory = new BookOrderFactory();

            BookOrderResult result = new BookOrderResult();
            using (var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113)))
            {
                connection.ConnectAsync().Wait();
                
                StreamEventsSlice currentSlice;
                long nextSliceStart = StreamPosition.Start;
                List<RecordedEvent> recordedEvents = new List<RecordedEvent>();
                do
                {
                    currentSlice = connection.ReadStreamEventsForwardAsync(
                        $"{StreamBaseName}-{orderId}", nextSliceStart,
                        200, false).Result;

                    nextSliceStart = currentSlice.NextEventNumber;

                    recordedEvents.AddRange(currentSlice.Events.Select(e => e.Event));                    
                } while (!currentSlice.IsEndOfStream);

                result.BookOrder = factory.Create(recordedEvents);
            }

            return result.BookOrder;
        }

        public EventData GetEventDataFor<T>(T item, string eventType)
        {
            var eventId = Guid.NewGuid();
            var eventDataFor = new EventData(eventId, eventType,
                isJson: true,
                data: GetSerializedBytes(item),
                metadata: GetSerializedBytes(new { }));

            return eventDataFor;
        }

        public byte[] GetSerializedBytes<U>(U input)
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(input));
        }
    }
}
