using System;
using System.Collections.Generic;
using Domain.Events;

namespace Domain.Entities
{
    public class BookOrder : AggregateBase
    {
        private BookOrder(string supplier, Guid id)
        {
            Supplier = supplier;
            Id = id;
            State = BookOrderState.New;
            AddEvent(new BookOrderCreatedEvent(supplier, id, State));
        }
        
        /// <summary>
        /// Create a new BookOrder
        /// </summary>
        public static BookOrder CreateNew(string supplier, Guid id)
        {
            return new BookOrder(supplier, id);
        }
        
        public string Supplier { get; }
        public Guid Id { get; }
        public BookOrderState State { get; private set; }        
    }
}