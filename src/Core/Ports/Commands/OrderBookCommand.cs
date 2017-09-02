using System;
using Core.UseCases;
using Core.ValueObjects;

namespace Core.Ports.Commands
{
    public class OrderBookCommand
    {
        private readonly OrderBookUseCase _orderBookUseCase;

        public OrderBookCommand(OrderBookUseCase orderBookUseCase)
        {
            if (orderBookUseCase == null) throw new ArgumentNullException(nameof(orderBookUseCase));
            _orderBookUseCase = orderBookUseCase;
        }

        public void Execute(BookRequest bookRequest)
        {
            _orderBookUseCase.Execute(bookRequest);
            Console.WriteLine($"Order book command executed for title: {bookRequest.Title}");
        }
    }
}