﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Ports.Persistence;
using Serilog;

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
            Log.Logger.Information("Retrieving BookOrderId: {BookOrderId}", id);
            if (_bookOrders.ContainsKey(id))
                return _bookOrders[id];

            return null;
        }

        public void Store(BookOrder bookOrder)
        {
            Log.Logger.Information("Store BookOrderId: {BookOrderId}", bookOrder.Id);
            _bookOrders[bookOrder.Id] = bookOrder;
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier)
        {
            return _bookOrders.Values.Where(x => x.Supplier == supplier);
        }

        public IEnumerable<BookOrder> GetBySupplier(string supplier, BookOrderState state)
        {
            return _bookOrders.Values.Where(x => x.Supplier == supplier &&
            x.State == state);
        }

        public IEnumerable<BookOrder> GetByState(BookOrderState state)
        {
            return _bookOrders.Values.Where(x => x.State == state);
        }

        public IEnumerable<BookOrder> Get()
        {
            return _bookOrders.Values;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}