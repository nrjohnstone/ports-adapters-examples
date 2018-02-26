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
        private Queue<BookTitleRequest> _testBookRequests;

        public OrderBookUseCaseTrigger(OrderBookUseCase orderBookUseCase)
        {
            if (orderBookUseCase == null) throw new ArgumentNullException(nameof(orderBookUseCase));
            _orderBookUseCase = orderBookUseCase;
            _testBookRequests = new Queue<BookTitleRequest>();
        }

        public void SetTestData(IEnumerable<BookTitleRequest> testData)
        {
            _testBookRequests = new Queue<BookTitleRequest>(testData);
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
                BookTitleRequest bookTitleRequest = _testBookRequests.Dequeue();
                _orderBookUseCase.Execute(bookTitleRequest);
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