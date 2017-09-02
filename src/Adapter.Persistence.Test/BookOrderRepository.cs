using System;
using System.Collections.Generic;
using AmbientContext.LogService.Serilog;
using Core.Entities;
using Core.Ports.Persistence;

namespace Adapter.Persistence.Test
{
    public class BookOrderRepository : IBookOrderRepository
    {
        public AmbientLogService Logger = new AmbientLogService();

        private readonly Dictionary<Guid, BookOrder> _bookOrders;

        public BookOrderRepository()
        {
            _bookOrders = new Dictionary<Guid, BookOrder>();
        }

        public BookOrder Get(Guid id)
        {
            Logger.Information("Retrieving BookOrderId: {BookOrderId}", id);
            if (_bookOrders.ContainsKey(id))
                return _bookOrders[id];

            throw new InvalidOperationException();
        }

        public void Store(Guid id, BookOrder bookOrder)
        {
            Logger.Information("Store BookOrderId: {BookOrderId}", id);
            _bookOrders[id] = bookOrder;
        }
    }
}