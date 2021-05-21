using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Ports.Persistence;

namespace Adapter.Persistence.InMemory
{
    public class BookOrderRepositoryInMemory : IBookOrderRepository
    {
        private List<BookOrder> _bookOrders = new List<BookOrder>();
        
        public BookOrder Get(Guid id)
        {
            return _bookOrders.FirstOrDefault(x => x.Id == id);
        }

        public void Store(BookOrder bookOrder)
        {
            var existingBookOrder = _bookOrders.FirstOrDefault(x => x.Id == bookOrder.Id);

            if (existingBookOrder != null)
            {
                _bookOrders.Remove(existingBookOrder);
            }
            
            _bookOrders.Add(bookOrder);
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier)
        {
            return _bookOrders.Where(x => x.Supplier == supplier);
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier, BookOrderState state)
        {
            return _bookOrders.Where(x => x.Supplier == supplier && x.State == state);
        }

        public IEnumerable<BookOrder> GetByState(BookOrderState state)
        {
            return _bookOrders.Where(x => x.State == state);
        }

        public IEnumerable<BookOrder> Get()
        {
            return _bookOrders;
        }

        public void Delete()
        {
            _bookOrders = new List<BookOrder>();
        }
    }
}
