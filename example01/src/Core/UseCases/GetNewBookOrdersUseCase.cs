using System;
using System.Collections.Generic;
using AmbientContext.LogService.Serilog;
using Domain.Entities;
using Domain.Ports.Persistence;

namespace Domain.UseCases
{
    public class GetNewBookOrdersUseCase
    {
        public AmbientLogService Logger = new AmbientLogService();
        private readonly IBookOrderRepository _bookOrderRepository;

        public GetNewBookOrdersUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null) throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public IEnumerable<BookOrder> Execute()
        {
            Logger.Information($"Execute {nameof(GetType)}");
            return _bookOrderRepository.GetByState(BookOrderState.New);
        }
    }
}