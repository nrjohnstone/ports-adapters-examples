using System.Threading;
using Domain.UseCases;
using Domain.ValueObjects;

namespace Adapter.Trigger.Csv
{
    internal class OrderBookUseCaseTrigger
    {
        private readonly OrderBookUseCase _orderBookUseCase;
        private readonly Thread _threadPoll;
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private bool _shutdown;

        public OrderBookUseCaseTrigger(OrderBookUseCase orderBookUseCase)
        {
            _orderBookUseCase = orderBookUseCase;
            _threadPoll = new Thread(PollForNewCsv);
        }

        private void PollForNewCsv()
        {
            while (!_shutdown)
            {
                BookTitleOrder bookTitleOrder = new BookTitleOrder(
                    "SomeTitle", "SomeSupplier", 10.5M, 5);
                _orderBookUseCase.Execute(bookTitleOrder);
                _shutdownEvent.WaitOne(5000);
            }
        }

        public void Start()
        {
            _threadPoll.Start();
        }

        public void Stop()
        {
            _shutdown = true;
            _shutdownEvent.Set();
        }
    }
}