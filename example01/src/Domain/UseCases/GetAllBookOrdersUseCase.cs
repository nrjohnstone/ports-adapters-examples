using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Ports.Persistence;
using Serilog;

namespace Domain.UseCases
{
    public class GetAllBookOrdersUseCase
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public GetAllBookOrdersUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null) throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public IEnumerable<BookOrder> Execute()
        {
            Log.Logger.Information($"Execute {nameof(GetType)}");
            return _bookOrderRepository.Get();
        }
    }
}