using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class BookOrder : AggregateBase
    {
        public IReadOnlyList<OrderLine> OrderLines => _orderLines;
        private readonly List<OrderLine> _orderLines;

        private BookOrder(string supplier, Guid id) : this(supplier, BookOrderState.New, id, new List<OrderLine>())
        {            
            AddEvent(new BookOrderCreatedEvent(supplier, id, State));
        }

        private BookOrder(string supplier, BookOrderState state, Guid id, IList<OrderLine> lines) 
        {
            Supplier = supplier;
            Id = id;
            State = state;
            _orderLines = lines.ToList();
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
        public static BookOrder CreateExisting(string supplier, BookOrderState state, Guid id, 
            IList<OrderLine> lines)
        {
            return new BookOrder(supplier, state, id, lines);
        }

        public void AddBookRequest(BookTitleOrder bookTitleOrder)
        {
            if (State != BookOrderState.New)
                throw new AddBookRequestException();

            if (Supplier != bookTitleOrder.Supplier)
                throw new InvalidOperationException("BookOrder is for different supplier");

            OrderLine orderLine = new OrderLine(bookTitleOrder.Title,
                bookTitleOrder.Price, bookTitleOrder.Quantity, Guid.NewGuid());

            _orderLines.Add(orderLine);
            AddEvent(new BookOrderLineCreatedEvent( orderLine.Title,  orderLine.Price,
                orderLine.Quantity, orderLine.Id, Id));
        }

        public string Supplier { get; }
        public Guid Id { get; }
        public BookOrderState State { get; private set; }

        public void UpdateOrderLinePrice(Guid orderLineId, decimal price)
        {
            var orderLine = OrderLines.FirstOrDefault(x => x.Id == orderLineId);

            if (orderLine == null)
                throw new InvalidOperationException("OrderLine does not exist");

            orderLine.UpdatePrice(price);

            AddEvent(new BookOrderLinePriceEditedEvent(Id, orderLine.Id, orderLine.Price));
        }

        public void RemoveOrderLine(Guid orderLineId)
        {
            var orderLine = OrderLines.FirstOrDefault(x => x.Id == orderLineId);

            if (orderLine == null)
                throw new InvalidOperationException("OrderLine does not exist");

            _orderLines.Remove(orderLine);
            AddEvent(new BookOrderLineRemovedEvent(orderLine.Id, Id));
        }

        public void CreateExistingOrderLine(OrderLine ol)
        {
            _orderLines.Add(ol);
        }
    }
}