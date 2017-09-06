using System;
using System.Collections.Generic;
using System.Threading;
using Core.UseCases;
using Core.ValueObjects;

namespace Adapter.Trigger.Test
{
    public class OrderBookUseCaseTrigger
    {
        private readonly OrderBookUseCase _orderBookUseCase;
        private Thread _producerThread;
        private bool _shutdown;
        private Queue<BookRequest> _testBookRequests;

        public OrderBookUseCaseTrigger(OrderBookUseCase orderBookUseCase)
        {
            if (orderBookUseCase == null) throw new ArgumentNullException(nameof(orderBookUseCase));
            _orderBookUseCase = orderBookUseCase;
            _testBookRequests = new Queue<BookRequest>();
        }

        public void SetTestData(IEnumerable<BookRequest> testData)
        {
            _testBookRequests = new Queue<BookRequest>(testData);
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
                BookRequest bookRequest = _testBookRequests.Dequeue();
                _orderBookUseCase.Execute(bookRequest);
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