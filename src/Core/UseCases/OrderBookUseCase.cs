using System;
using System.Collections.Generic;
using System.Linq;
using AmbientContext.LogService.Serilog;
using Core.Entities;
using Core.Ports.Persistence;
using Core.ValueObjects;

namespace Core.UseCases
{
    public class OrderBookUseCase
    {
        public AmbientLogService Logger = new AmbientLogService();
        private readonly IBookOrderRepository _bookOrderRepository;

        public OrderBookUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null)
                throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public void Execute(BookRequest bookRequest)
        {
            Logger.Information("Execute OrderBookUseCase for Title: {Title}", bookRequest.Title);

            IEnumerable<BookOrder> bookOrders = _bookOrderRepository.GetBySupplier(bookRequest.Supplier);
            var bookOrder = bookOrders.FirstOrDefault();

            if (bookOrder == null)
            {
                bookOrder = new BookOrder(
                    bookRequest.Supplier, Guid.NewGuid(), BookOrderState.Pending);
            }
           
            bookOrder.AddBookRequest(bookRequest);
            
            _bookOrderRepository.Store(bookOrder);
        }
    }
}