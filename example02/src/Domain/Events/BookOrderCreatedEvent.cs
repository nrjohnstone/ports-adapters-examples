using System;
using Domain.Entities;

namespace Domain.Events
{
    public class BookOrderCreatedEvent : IEvent
    {
        public string Supplier { get; }
        public Guid Id { get; }

        public BookOrderCreatedEvent(string supplier, Guid id, BookOrderState state)
        {
            Supplier = supplier;
            Id = id;
        }

        public string GetEventType() => EventType;      
        public static string EventType => "BookOrderCreated";
    }
}