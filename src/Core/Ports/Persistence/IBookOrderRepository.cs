using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Ports.Persistence
{
    public interface IBookOrderRepository
    {
        BookOrder Get(Guid id);
        void Store(BookOrder bookOrder);
        IEnumerable<BookOrder> GetBySupplier(string supplier);
        IEnumerable<BookOrder> GetBySupplier(string supplier, BookOrderState state);
        IEnumerable<BookOrder> GetByState(BookOrderState state);
    }
}