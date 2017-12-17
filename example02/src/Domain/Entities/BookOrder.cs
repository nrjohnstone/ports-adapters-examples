using System;
using System.Collections.Generic;
using Domain.Events;

namespace Domain.Entities
{
    public class BookOrder : AggregateBase
    {
        private BookOrder(string supplier, Guid id) :this(supplier, BookOrderState.New, id)
        {            
            AddEvent(new BookOrderCreatedEvent(supplier, id, State));
        }

        private BookOrder(string supplier, BookOrderState state, Guid id) 
        {
            Supplier = supplier;
            Id = id;
            State = state;
        }
        
        /// <summary>
        /// Create a new BookOrder
        /// </summary>
        public static BookOrder CreateNew(string supplier, Guid id)
        {
            return new BookOrder(supplier, id);
        }

        /// <summary>
        /// Create an existing book order
        /// </summary>
        public static BookOrder CreateExisting(string supplier, BookOrderState state, Guid id)
        {
            return new BookOrder(supplier, state, id);
        }

        public string Supplier { get; }
        public Guid Id { get; }
        public BookOrderState State { get; private set; }

    }
}