using System.Collections.Generic;
using Core.Ports.Commands;
using Core.ValueObjects;

namespace Adapter.Command
{
    public class TestCommandAdapter
    {
        private OrderBookCommandHandler _orderBookCommandHandler;

        public void Initialize()
        {
            
        }

        public void Handle(OrderBookCommand orderBookCommand,
            IEnumerable<BookRequest> testData)
        {
            _orderBookCommandHandler = new OrderBookCommandHandler(
                orderBookCommand);

            _orderBookCommandHandler.SetTestData(testData);
            _orderBookCommandHandler.Start();
        }

        public void Shutdown()
        {
            _orderBookCommandHandler.Stop();
        }
    }
}