using System;
using Core.Ports.Persistence;
using Core.ValueObjects;

namespace Core.UseCases
{
    public class OrderBookUseCase
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public OrderBookUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null)
                throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public void Execute(BookRequest bookRequest)
        {

        }
    }
}