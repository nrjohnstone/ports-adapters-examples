using System;

namespace Core.Entities
{
    public class BookOrder
    {
        public BookOrder(string supplier, Guid id)
        {
            if (string.IsNullOrWhiteSpace(supplier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(supplier));

            Supplier = supplier;
            Id = id;
        }

        public string Supplier { get; }
        public Guid Id { get; }
    }
}