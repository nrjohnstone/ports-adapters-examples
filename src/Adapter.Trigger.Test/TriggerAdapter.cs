using System;
using System.Collections.Generic;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Adapter.Trigger.Test
{
    public class TriggerAdapter
    {
        private OrderBookUseCaseRandomTrigger _orderBookUseCaseTrigger;
        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;
        }

        public void Handle(OrderBookUseCase orderBookUseCase)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            IEnumerable<BookTitleOrder> testData = new[] {
                new BookTitleOrder(title: "The Light Fantastic", supplier: "Acme Inc",  price: 15M, quantity: 1),
                new BookTitleOrder(title: "The Blind Watchmaker", supplier: "Winston Publishing",  price: 24.99M, quantity: 10),
                new BookTitleOrder(title: "Dirk Gently", supplier: "Acme Inc", price: 10.99M, quantity: 2)
            };
            
            _orderBookUseCaseTrigger = new OrderBookUseCaseRandomTrigger(orderBookUseCase);

            //_orderBookUseCaseTrigger.SetTestData(testData);
            _orderBookUseCaseTrigger.Start();
        }

        public void Shutdown()
        {
            _orderBookUseCaseTrigger.Stop();
        }
    }
}