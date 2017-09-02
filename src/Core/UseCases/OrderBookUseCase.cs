using System;
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

            BookOrder bookOrder = new BookOrder(
                "Acme Inc", Guid.NewGuid());
            
            _bookOrderRepository.Store(bookOrder);
        }
    }
}