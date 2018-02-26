using System;
using System.Collections.Generic;
using System.Threading;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Adapter.Trigger.Test
{
    public class OrderBookUseCaseTrigger
    {
        private readonly AddBookTitleRequestUseCase _addBookTitleRequestUseCase;
        private Thread _producerThread;
        private bool _shutdown;
        private Queue<BookTitleRequest> _testBookRequests;

        public OrderBookUseCaseTrigger(AddBookTitleRequestUseCase addBookTitleRequestUseCase)
        {
            if (addBookTitleRequestUseCase == null) throw new ArgumentNullException(nameof(addBookTitleRequestUseCase));
            _addBookTitleRequestUseCase = addBookTitleRequestUseCase;
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
                _addBookTitleRequestUseCase.Execute(bookTitleRequest);
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