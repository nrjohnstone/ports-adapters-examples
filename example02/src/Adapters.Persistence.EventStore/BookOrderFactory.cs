using System.Collections.Generic;
using Adapters.Persistence.EventStore.EventHandlers;
using Domain.Entities;
using EventStore.ClientAPI;

namespace Adapters.Persistence.EventStore
{
    internal class BookOrderFactory
    {
        private readonly List<IBookOrderEventHandler> _bookOrderEventHandlers;

        public BookOrderFactory()
        {
            _bookOrderEventHandlers = new List<IBookOrderEventHandler>
            {
                new BookOrderCreatedEventHandler(),
                new BookOrderLineCreatedEventHandler(),
                new BookOrderLinePriceEditedEventHandler(),
                new BookOrderLineRemovedEventHandler()
            };
        }

        public BookOrder Create(IEnumerable<RecordedEvent> recordedEvents)
        {
            BookOrderResult result = new BookOrderResult();

            foreach (var recordedEvent in recordedEvents)
            {
                foreach (var handler in _bookOrderEventHandlers)
                {
                    if (handler.CanHandle(recordedEvent))
                    {
                        handler.Handle(recordedEvent, result);
                        break;
                    }
                }
            }

            return result.BookOrder;
        }
    }
}