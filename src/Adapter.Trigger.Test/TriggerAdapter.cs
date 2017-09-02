using System;
using System.Collections.Generic;
using Core.UseCases;
using Core.ValueObjects;

namespace Adapter.Command
{
    public class TriggerAdapter
    {
        private OrderBookUseCaseTrigger _orderBookUseCaseTrigger;
        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;
        }

        public void Handle(OrderBookUseCase orderBookUseCase)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            IEnumerable<BookRequest> testData = new[] {
                new BookRequest(title: "The Light Fantastic", supplier: "Acme Inc",  price: 15M, quantity: 1),
                new BookRequest(title: "The Blind Watchmaker", supplier: "Winston Publishing",  price: 24.99M, quantity: 10),
                new BookRequest(title: "Dirk Gently", supplier: "Acme Inc", price: 10.99M, quantity: 2)
            };
            
            _orderBookUseCaseTrigger = new OrderBookUseCaseTrigger(orderBookUseCase);

            _orderBookUseCaseTrigger.SetTestData(testData);
            _orderBookUseCaseTrigger.Start();
        }

        public void Shutdown()
        {
            _orderBookUseCaseTrigger.Stop();
        }
    }
}