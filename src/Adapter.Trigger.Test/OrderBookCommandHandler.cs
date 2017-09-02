using System;
using System.Collections.Generic;
using System.Threading;
using Core.Ports.Commands;
using Core.ValueObjects;

namespace Adapter.Command
{
    public class OrderBookCommandHandler
    {
        private readonly OrderBookCommand _orderBookCommand;
        private Thread _producerThread;
        private bool _shutdown;
        private Queue<BookRequest> _testBookRequests;

        public OrderBookCommandHandler(OrderBookCommand orderBookCommand)
        {
            if (orderBookCommand == null) throw new ArgumentNullException(nameof(orderBookCommand));
            _orderBookCommand = orderBookCommand;
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

                _orderBookCommand.Execute(bookRequest);
                Thread.Sleep(1000);
            }
        }

        private Queue<BookRequest> InitializeTestData()
        {
            return new Queue<BookRequest>(new []
            {
                new BookRequest(title: "The Light Fantastic", supplier: "Osborne", price:15M, quantity: 1), 
                new BookRequest("The Blind Watchmaker", supplier: "Osborne", price:24.99M, quantity: 10), 
                new BookRequest("Dirk Gently", supplier: "Osborne", price: 10.99M, quantity: 2) 
            });
        }

        public void Stop()
        {
            _shutdown = true;
            if (!_producerThread.Join(500))
                _producerThread.Abort();
        }
    }
}