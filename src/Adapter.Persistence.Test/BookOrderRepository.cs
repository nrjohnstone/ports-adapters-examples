using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Ports.Persistence;

namespace Adapter.Persistence.Test
{
    public class BookOrderRepository : IBookOrderRepository
    {
        private readonly Dictionary<Guid, BookOrder> _bookOrders;

        public BookOrderRepository()
        {
            _bookOrders = new Dictionary<Guid, BookOrder>();
        }

        public BookOrder Get(Guid id)
        {
            if (_bookOrders.ContainsKey(id))
                return _bookOrders[id];

            throw new InvalidOperationException();
        }

        public void Store(Guid id, BookOrder bookOrder)
        {
            _bookOrders[id] = bookOrder;
        }
    }
}