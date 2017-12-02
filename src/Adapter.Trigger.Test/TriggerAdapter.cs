﻿using System;
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
            
            _orderBookUseCaseTrigger = new OrderBookUseCaseRandomTrigger(orderBookUseCase);
            _orderBookUseCaseTrigger.Start();
        }

        public void Shutdown()
        {
            _orderBookUseCaseTrigger.Stop();
        }
    }
}