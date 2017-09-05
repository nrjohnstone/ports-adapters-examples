using System;
using Core.Entities;
using Fluency;

namespace Core.Tests.Unit.Helpers
{
    public class BookOrderBuilder : FluentBuilder<BookOrder>
    {
        private string _supplier = "BookSupplier";
        private Guid _id = Guid.NewGuid();
        private BookOrderState _state = BookOrderState.New;

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
            return new BookOrder(_supplier, _id, _state);
        }

        public BookOrderBuilder InState(BookOrderState bookOrderState)
        {            
            _state = bookOrderState;
            return this;
        }
    }
}