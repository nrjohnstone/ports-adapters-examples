using Domain.ValueObjects;
using Fluency;

namespace Core.Tests.Unit.Helpers
{
    public class BookTitleOrderBuilder : FluentBuilder<BookTitleOrder>
    {
        private string _title = "BookTitle";
        private string _supplier = "BookSupplier";
        private decimal _price = 10.5M;
        private int _quantity = 1;

        public BookTitleOrderBuilder()
        {
            IgnoreAllProperties();
        }

        public static implicit operator BookTitleOrder(BookTitleOrderBuilder builder)
        {
            return builder.build();
        }

        protected override BookTitleOrder GetNewInstance()
        {
            return new BookTitleOrder(_title, _supplier, _price, _quantity);
        }

        public BookTitleOrderBuilder ForSupplier(string supplier)
        {
            _supplier = supplier;
            return this;
        }
        
        public BookTitleOrder ForTitle(string title)
        {
            _title = title;
            return this;
        }
    }
}