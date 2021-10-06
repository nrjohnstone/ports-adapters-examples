using System;
using System.Collections.Generic;
using Domain.Entities;
using Fluency;

namespace Core.Tests.Unit.Helpers
{
    public class BookOrderBuilder : FluentBuilder<BookOrder>
    {
        private string _supplier = "BookSupplier";
        private Guid _id = Guid.NewGuid();
        private Nullable<BookOrderState> _state = null;
        private IEnumerable<OrderLine> _orderLines = new List<OrderLine>();

        public BookOrderBuilder()
        {
            IgnoreAllProperties();
        }

        public static implicit operator BookOrder(BookOrderBuilder builder)
        {
            return builder.build();
        }

        protected override BookOrder GetNewInstance()
        {
            // Default BookOrder is a new one
            if (_state == null || _state == BookOrderState.New)
                return BookOrder.CreateNew(_supplier, _id);

            if (_state == BookOrderState.Approved)
            {
                var bookOrder = BookOrder.CreateNew(_supplier, _id);
                bookOrder.Approve();
                return bookOrder;
            }

            return BookOrder.CreateExisting(_supplier, _id, _state.Value, _orderLines);
        }

        public BookOrderBuilder ThatIsNew()
        {
            if (_state != null)
                throw new InvalidOperationException("The state of a book order should only be specified once");

            _state = BookOrderState.New;
            return this;
        }

        public BookOrderBuilder ThatIsApproved()
        {
            if (_state != null)
                throw new InvalidOperationException("The state of a book order should only be specified once");

            _state = BookOrderState.Approved;
            return this;
        }

        public BookOrderBuilder ThatIsSent()
        {
            if (_state != null)
                throw new InvalidOperationException("The state of a book order should only be specified once");

            _state = BookOrderState.Sent;
            return this;
        }

        public BookOrderBuilder WithState(BookOrderState bookOrderState)
        {
            _state = bookOrderState;
            return this;
        }
    }
}