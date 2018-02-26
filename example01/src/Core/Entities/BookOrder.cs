using System;
using System.Collections.Generic;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class BookOrder
    {
        public BookOrder(string supplier, Guid id, BookOrderState state)
        {
            if (string.IsNullOrWhiteSpace(supplier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(supplier));

            Supplier = supplier;
            Id = id;
            State = state;

            OrderLines = new List<OrderLine>();
        }

        public BookOrder(string supplier, Guid id, BookOrderState state, IEnumerable<OrderLine> orderLines)
        {
            if (string.IsNullOrWhiteSpace(supplier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(supplier));

            Supplier = supplier;
            Id = id;
            State = state;

            OrderLines = new List<OrderLine>();
            OrderLines.AddRange(orderLines);
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