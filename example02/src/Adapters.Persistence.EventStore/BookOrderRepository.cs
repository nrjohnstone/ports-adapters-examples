using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Domain.Entities;
using Domain.Events;
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
            BookOrder bookOrder = null;
            using (var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113)))
            {
                connection.ConnectAsync().Wait();

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
                        if (currentSliceEvent.Event.EventType.Equals(BookOrderCreatedEvent.EventType))
                        {
                            var st = Encoding.ASCII.GetString(currentSliceEvent.Event.Data);
                            BookOrderCreatedEvent ev = JsonConvert.DeserializeObject< BookOrderCreatedEvent>(st);
                            bookOrder = BookOrder.CreateExisting(ev.Supplier, BookOrderState.New, ev.Id, 
                                new List<OrderLine>());
                        }
                        else if (currentSliceEvent.Event.EventType.Equals(BookOrderLineCreatedEvent.EventType))
                        {
                            var st = Encoding.ASCII.GetString(currentSliceEvent.Event.Data);
                            BookOrderLineCreatedEvent ev = 
                                JsonConvert.DeserializeObject<BookOrderLineCreatedEvent>(st);

                            OrderLine ol = new OrderLine(
                                ev.Title, ev.Price, ev.Quantity, ev.OrderLineId);
                            bookOrder.CreateExistingOrderLine(ol);
                        }
                        else if (currentSliceEvent.Event.EventType.Equals(BookOrderLinePriceEditedEvent.EventType))
                        {
                            var st = Encoding.ASCII.GetString(currentSliceEvent.Event.Data);
                            BookOrderLinePriceEditedEvent ev = 
                                JsonConvert.DeserializeObject<BookOrderLinePriceEditedEvent>(st);

                            bookOrder.UpdateOrderLinePrice(ev.OrderLineId, ev.Price);
                        }
                    }
                    
                } while (!currentSlice.IsEndOfStream);
            }

            return bookOrder;
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