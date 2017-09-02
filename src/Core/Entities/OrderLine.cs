using System;

namespace Core.Entities
{
    public class OrderLine
    {
        public string Title { get; }
        public decimal Price { get; }
        public int Quantity { get; }

        public OrderLine(string title, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            Title = title;
            Price = price;
            Quantity = quantity;
        }
    }
}