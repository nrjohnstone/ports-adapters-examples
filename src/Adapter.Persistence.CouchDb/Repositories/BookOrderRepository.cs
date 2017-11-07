using System;
using System.Collections.Generic;
using Adapter.Persistence.CouchDb.Repositories.Dtos;
using Domain.Entities;
using Domain.Ports.Persistence;
using MyCouch;
using Newtonsoft.Json;

namespace Adapter.Persistence.CouchDb.Repositories
{
    public class BookOrderRepository : IBookOrderRepository
    {
        string _httpLocalhost = "http://admin:123@localhost:5984";

        public BookOrder Get(Guid id)
        {
            BookOrderDto retrieved;
            
            using (var store = new MyCouchStore(_httpLocalhost, "mydb"))
            {
                var doc = store.GetByIdAsync(id.ToString()).Result;

                if (doc == null)
                    return null;

                retrieved = JsonConvert.DeserializeObject<BookOrderDto>(doc);
            }

            return BookOrderMapper.MapFrom(retrieved);
        }

        public void Store(BookOrder bookOrder)
        {
            var id = bookOrder.Id.ToString();

            using (var store = new MyCouchStore(_httpLocalhost, "mydb"))
            {
                // HACK: Concurrency support, lets just get the latest rev
                // Don't do this in production environments !! Implement concurrency properly all the way
                // to the client !!
                string rev = GetRev(store, bookOrder.Id);
                
                BookOrderDto bookOrderDto = BookOrderMapper.MapTo(bookOrder, rev);
                var doc = JsonConvert.SerializeObject(bookOrderDto);
                
                if (rev == null)
                {
                    store.StoreAsync(id, doc).Wait();
                }
                else
                {
                    store.StoreAsync(id, rev, doc).Wait();
                }                    
            }
        }

        private string GetRev(MyCouchStore store, Guid id)
        {
            var doc = store.GetByIdAsync(id.ToString()).Result;

            if (doc == null)
                return null;

            var retrieved = store.GetByIdAsync<BookOrderDto>(id.ToString()).Result;

            //var retrieved = JsonConvert.DeserializeObject<BookOrderDto>(doc);
            return retrieved.Rev;
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