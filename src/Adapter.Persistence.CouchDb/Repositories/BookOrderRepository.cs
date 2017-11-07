using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Ports.Persistence;
using MyCouch;

namespace Adapter.Persistence.CouchDb.Repositories
{
    public class BookOrderRepository : IBookOrderRepository
    {
        public BookOrder Get(Guid id)
        {
            BookOrder retrieved;
            using (var store = new MyCouchStore("http://localhost:5984", "mydb"))
            {
                retrieved = store.GetByIdAsync<BookOrder>(id.ToString()).Result;
            }
            return retrieved;
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

        public IEnumerable<BookOrder> Get()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}