using System.Collections.Generic;
using System.Threading;
using Adapter.Command;
using Core.Ports.Commands;
using Core.UseCases;
using Core.ValueObjects;

namespace Host.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderBookCommand = new OrderBookCommand(
                new OrderBookUseCase());

            var commandAdapter = new TestCommandAdapter();

            commandAdapter.Initialize();
            IEnumerable<BookRequest> testData = new[] {
                new BookRequest(title: "The Light Fantastic", supplier: "Acme Inc",  price: 15M, quantity: 1),
                new BookRequest(title: "The Blind Watchmaker", supplier: "Winston Publishing",  price: 24.99M, quantity: 10),
                new BookRequest(title: "Dirk Gently", supplier: "Acme Inc", price: 10.99M, quantity: 2)
            };

            commandAdapter.Handle(orderBookCommand, testData);

            Thread.Sleep(5000);

            commandAdapter.Shutdown();
        }
    }
}
