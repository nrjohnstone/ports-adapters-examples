using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Adapters.Persistence.EventStore.EventHandlers;
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
            BookOrderResult result = new BookOrderResult();
            using (var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113)))
            {
                connection.ConnectAsync().Wait();
                List<IBookOrderEventHandler> bookOrderEventHandlers = new List<IBookOrderEventHandler>();
                bookOrderEventHandlers.Add(new BookOrderCreatedEventHandler());
                bookOrderEventHandlers.Add(new BookOrderLineCreatedEventHandler());
                bookOrderEventHandlers.Add(new BookOrderLinePriceEditedEventHandler());
                StreamEventsSlice currentSlice;
                long nextSliceStart = StreamPosition.Start;
                do
                {
                    currentSlice = connection.ReadStreamEventsForwardAsync(
                        $"{StreamBaseName}-{orderId}", nextSliceStart,
                        200, false).Result;

                    nextSliceStart = currentSlice.NextEventNumber;

                    foreach (var currentSliceEvent in currentSlice.Events)
                    {
                        foreach (var handler in bookOrderEventHandlers)
                        {
                            if (handler.CanHandle(currentSliceEvent.Event))
                            {
                                handler.Handle(currentSliceEvent.Event, result);
                                break;
                            }
                        }
                    }

                } while (!currentSlice.IsEndOfStream);
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
