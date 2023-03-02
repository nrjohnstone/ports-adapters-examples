using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Ports.Persistence;
using Serilog;

namespace Domain.UseCases
{
    public class GetAllNewBookOrdersUseCase
    {
        private readonly IBookOrderRepository _bookOrderRepository;

        public GetAllNewBookOrdersUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null) throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public IEnumerable<BookOrder> Execute()
        {
            Log.Logger.Information($"Execute {nameof(GetType)}");
            return _bookOrderRepository.GetByState(BookOrderState.New);
        }
    }
}