using Core.ValueObjects;
using Fluency;

namespace Core.Tests.Unit.Helpers
{
    public class BookRequestBuilder : FluentBuilder<BookRequest>
    {
        private string _title = "BookTitle";
        private string _supplier = "BookSupplier";
        private decimal _price = 10.5M;
        private int _quantity = 1;

        public static implicit operator BookRequest(BookRequestBuilder builder)
        {
            return builder.build();
        }

        protected override BookRequest GetNewInstance()
        {
            return new BookRequest(_title, _supplier, _price, _quantity);
        }

        public BookRequestBuilder ForSupplier(string supplier)
        {
            _supplier = supplier;
            return this;
        }

        public BookRequest ForTitle(string title)
        {
            _title = title;
            return this;
        }
    }
}