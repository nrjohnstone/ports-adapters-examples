using System;
using System.Collections.Generic;
using System.Linq;
using Adapter.Persistence.InMemory.Dtos;
using Domain.Entities;
using Domain.Ports.Persistence;

namespace Adapter.Persistence.InMemory
{
    public class BookOrderRepositoryInMemory : IBookOrderRepository
    {
        private List<BookOrderDto> _bookOrders = new List<BookOrderDto>();
        
        public BookOrder Get(Guid id)
        {
            var bookOrderDto = _bookOrders.FirstOrDefault(x => x.Id == id);

            if (bookOrderDto == null)
            {
                return null;
            }

            return bookOrderDto.ToDomain();
        }

        public void Store(BookOrder bookOrder)
        {
            var existingBookOrder = _bookOrders.FirstOrDefault(x => x.Id == bookOrder.Id);

            if (existingBookOrder != null)
            {
                _bookOrders.Remove(existingBookOrder);
            }
            
            _bookOrders.Add(bookOrder.ToDto());
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier)
        {
            return _bookOrders.Where(x => x.Supplier == supplier).ToDomain();
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier, BookOrderState state)
        {
            return _bookOrders.Where(x => x.Supplier == supplier && x.State == state.ToDto()).ToDomain();
        }

        public IEnumerable<BookOrder> GetByState(BookOrderState state)
        {
            return _bookOrders.Where(x => x.State == state.ToDto()).ToDomain();
        }

        public IEnumerable<BookOrder> Get()
        {
            return _bookOrders.ToDomain();
        }

        public void Delete()
        {
            _bookOrders = new List<BookOrderDto>();
        }
    }
}
