using System.Collections.Generic;
using System.Linq;
using Domain.Events;

namespace Domain.Entities
{
    public abstract class AggregateBase
    {
        private readonly List<IEvent> _events = new List<IEvent>();

        public IReadOnlyCollection<IEvent> DequeueAllEvents()
        {
            var allEvents = _events.ToList();

            _events.Clear();

            return allEvents;
        }

        protected void AddEvent(IEvent @event)
        {
            _events.Add(@event);
        }
    }
}