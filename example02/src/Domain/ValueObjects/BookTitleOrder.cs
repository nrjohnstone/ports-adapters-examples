using System;

namespace Domain.ValueObjects
{
    public class BookTitleOrder
    {
        public BookTitleOrder(string title, string supplier, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price));
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));
            if (string.IsNullOrWhiteSpace(supplier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(supplier));

            Title = title;
            Price = price;
            Quantity = quantity;
            Supplier = supplier;
        }

        public string Supplier { get; }
        public string Title { get; }
        public decimal Price { get; }
        public int Quantity { get; }
    }
}