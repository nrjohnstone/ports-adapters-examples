using System;
using System.Collections.Generic;
using System.Threading;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Adapter.Trigger.Test
{
    public class OrderBookUseCaseTrigger
    {
        private readonly OrderBookUseCase _orderBookUseCase;
        private Thread _producerThread;
        private bool _shutdown;
        private Queue<BookTitleOrder> _testBookRequests;

        public OrderBookUseCaseTrigger(OrderBookUseCase orderBookUseCase)
        {
            if (orderBookUseCase == null) throw new ArgumentNullException(nameof(orderBookUseCase));
            _orderBookUseCase = orderBookUseCase;
            _testBookRequests = new Queue<BookTitleOrder>();
        }

        public void SetTestData(IEnumerable<BookTitleOrder> testData)
        {
            _testBookRequests = new Queue<BookTitleOrder>(testData);
        }

        public void Start()
        {
            _producerThread = new Thread(ProducerCycle);
            _producerThread.Start();
        }

        private void ProducerCycle()
        {
            while (!_shutdown && _testBookRequests.Count > 0)
            {
                BookTitleOrder bookTitleOrder = _testBookRequests.Dequeue();
                _orderBookUseCase.Execute(bookTitleOrder);
                Thread.Sleep(5000);
            }
        }

        public void Stop()
        {
            _shutdown = true;
            if (!_producerThread.Join(500))
                _producerThread.Abort();
        }
    }
}