using System;
using System.Collections.Generic;
using System.Linq;
using AmbientContext.LogService.Serilog;
using Domain.Entities;
using Domain.Ports.Persistence;
using Domain.ValueObjects;

namespace Domain.UseCases
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

        public Guid Execute(BookTitleOrder bookTitleOrder)
        {
            Logger.Information("Execute OrderBookUseCase for Title: {Title}", bookTitleOrder.Title);

            IEnumerable<BookOrder> bookOrders = _bookOrderRepository.GetBySupplier(
                bookTitleOrder.Supplier, BookOrderState.New);
            var bookOrder = bookOrders.FirstOrDefault();

            if (bookOrder == null)
            {
                bookOrder = new BookOrder(
                    bookTitleOrder.Supplier, Guid.NewGuid(), BookOrderState.New);
            }
           
            bookOrder.AddBookRequest(bookTitleOrder);
            
            _bookOrderRepository.Store(bookOrder);

            return bookOrder.Id;
        }
    }
}