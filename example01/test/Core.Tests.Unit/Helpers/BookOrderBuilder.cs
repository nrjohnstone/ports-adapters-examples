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
        private BookOrderState _state = BookOrderState.New;
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
            if (_state == BookOrderState.New)
                return BookOrder.CreateNew(_supplier, _id);
            else
            {
                return BookOrder.CreateExisting(_supplier, _id, _state, _orderLines);
            }
        }

        public BookOrderBuilder InState(BookOrderState bookOrderState)
        {
            _state = bookOrderState;
            return this;
        }
    }
}