using System;
using System.Collections.Generic;
using System.Linq;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class BookOrder
    {
        private BookOrder(string supplier, Guid id, BookOrderState state, IEnumerable<OrderLine> orderLines)
        {
            if (string.IsNullOrWhiteSpace(supplier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(supplier));

            if (orderLines == null)
                throw new ArgumentNullException(nameof(orderLines));

            Supplier = supplier;
            Id = id;
            State = state;

            OrderLines = orderLines.ToList();
        }

        public static BookOrder CreateNew(string supplier, Guid id)
        {
            return new BookOrder(supplier, id, BookOrderState.New, new List<OrderLine>());
        }

        public static BookOrder CreateExisting(string supplier, Guid id, BookOrderState state,
            IEnumerable<OrderLine> orderLines)
        {
            return new BookOrder(supplier, id, state, orderLines);
        }

        public void AddBookRequest(BookTitleRequest bookTitleRequest)
        {
            if (State != BookOrderState.New)
                throw new AddBookRequestException();

            OrderLine orderLine = new OrderLine(bookTitleRequest.Title,
                bookTitleRequest.Price, bookTitleRequest.Quantity, Guid.NewGuid());

            OrderLines.Add(orderLine);
        }

        public string Supplier { get; }
        public Guid Id { get; }
        public List<OrderLine> OrderLines { get; }
        public BookOrderState State { get; private set; }

        public void Send()
        {
            if (State != BookOrderState.Approved)
                throw new BookOrderSendException();

            State = BookOrderState.Sent;
        }

        public void Approve()
        {
            if (State != BookOrderState.New)
                throw new BookOrderApproveException();

            State = BookOrderState.Approved;
        }
    }
}