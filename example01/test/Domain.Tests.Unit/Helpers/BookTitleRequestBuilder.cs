using Domain.ValueObjects;
using Fluency;

namespace Core.Tests.Unit.Helpers
{
    public class BookTitleRequestBuilder : FluentBuilder<BookTitleRequest>
    {
        private string _title = "BookTitle";
        private string _supplier = "BookSupplier";
        private decimal _price = 10.5M;
        private int _quantity = 1;

        public BookTitleRequestBuilder()
        {
            IgnoreAllProperties();
        }

        public static implicit operator BookTitleRequest(BookTitleRequestBuilder builder)
        {
            return builder.build();
        }

        protected override BookTitleRequest GetNewInstance()
        {
            return new BookTitleRequest(_title, _supplier, _price, _quantity);
        }

        public BookTitleRequestBuilder ForSupplier(string supplier)
        {
            _supplier = supplier;
            return this;
        }

        public BookTitleRequestBuilder ForTitle(string title)
        {
            _title = title;
            return this;
        }

        public BookTitleRequestBuilder WithPrice(decimal price)
        {
            _price = price;
            return this;
        }

        public BookTitleRequestBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }
    }
}