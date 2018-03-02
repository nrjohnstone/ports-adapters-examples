using System;
using System.Globalization;
using System.Threading;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Adapter.Trigger.Test
{
    public class OrderBookUseCaseRandomTrigger
    {
        private readonly AddBookTitleRequestUseCase _addBookTitleRequestUseCase;
        private Thread _producerThread;
        private bool _shutdown;

        public OrderBookUseCaseRandomTrigger(AddBookTitleRequestUseCase addBookTitleRequestUseCase)
        {
            if (addBookTitleRequestUseCase == null) throw new ArgumentNullException(nameof(addBookTitleRequestUseCase));
            _addBookTitleRequestUseCase = addBookTitleRequestUseCase;
        }
     
        public void Start()
        {
            _producerThread = new Thread(ProducerCycle);
            _producerThread.Start();
        }

        private void ProducerCycle()
        {
            Random rand = new Random();

            while (!_shutdown)
            {
                string supplier = $"Supplier{rand.Next(1, 5)}";
                int numberOfRequestsForSupplier = rand.Next(1, 5);

                for (int i = 0; i < numberOfRequestsForSupplier; i++)
                {                    
                    Decimal price = rand.Next(1, 100);
                    int quantity = rand.Next(1, 10);

                    BookTitleRequest bookTitleRequest = new BookTitleRequest(
                        $"Title{Guid.NewGuid().ToString()}",
                        supplier,
                        price, quantity);
                    _addBookTitleRequestUseCase.Execute(bookTitleRequest);
                }
                
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