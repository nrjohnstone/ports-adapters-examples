using System;
using System.Collections.Generic;
using Core.Ports.Commands;
using Core.ValueObjects;

namespace Adapter.Command
{
    public class TriggerAdapter
    {
        private OrderBookCommandHandler _orderBookCommandHandler;
        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;
        }

        public void Handle(OrderBookCommand orderBookCommand,
            IEnumerable<BookRequest> testData)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            _orderBookCommandHandler = new OrderBookCommandHandler(orderBookCommand);

            _orderBookCommandHandler.SetTestData(testData);
            _orderBookCommandHandler.Start();
        }

        public void Shutdown()
        {
            _orderBookCommandHandler.Stop();
        }
    }
}