using System;
using System.Collections.Generic;
using AmbientContext.LogService.Serilog;
using Core.Entities;
using Core.Ports.Persistence;

namespace Core.UseCases
{
    public class GetBookOrdersUseCase
    {
        public AmbientLogService Logger = new AmbientLogService();
        private readonly IBookOrderRepository _bookOrderRepository;

        public GetBookOrdersUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null) throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public IEnumerable<BookOrder> Execute()
        {
            Logger.Information("Execute GetBookOrdersUseCase");
            return _bookOrderRepository.Get();
        }
    }
}