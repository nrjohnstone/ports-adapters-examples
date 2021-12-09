using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Domain.UseCases;

namespace Adapter.Trigger.Csv
{
    public class TriggerAdapter
    {
        private bool _initialized;
        private OrderBookUseCaseTrigger _orderBookUseCaseTrigger;

        public void Initialize()
        {
            _initialized = true;
        }

        /// <summary>
        /// Wire the instance of the AddBookTitleRequestUseCase to a trigger that will poll and
        /// call the use case when matching trigger conditions are met
        /// </summary>
        /// <param name="addBookTitleRequestUseCase"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Handle(AddBookTitleRequestUseCase addBookTitleRequestUseCase)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            _orderBookUseCaseTrigger = new OrderBookUseCaseTrigger(addBookTitleRequestUseCase);
            _orderBookUseCaseTrigger.Start();
        }

        public void Shutdown()
        {
            _orderBookUseCaseTrigger.Stop();
        }
    }
}
