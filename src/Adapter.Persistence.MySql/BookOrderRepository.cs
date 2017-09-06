using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Ports.Persistence;

namespace Adapter.Persistence.MySql
{
    public class BookOrderRepository : IBookOrderRepository
    {
        public BookOrder Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Store(BookOrder bookOrder)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier, BookOrderState state)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BookOrder> GetByState(BookOrderState state)
        {
            throw new NotImplementedException();
        }
    }
}