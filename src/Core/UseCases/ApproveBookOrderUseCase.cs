using System;
using AmbientContext.LogService.Serilog;
using Domain.Entities;
using Domain.Ports.Persistence;

namespace Domain.UseCases
{
    public class ApproveBookOrderUseCase
    {
        public AmbientLogService Logger = new AmbientLogService();
        private readonly IBookOrderRepository _bookOrderRepository;

        public ApproveBookOrderUseCase(IBookOrderRepository bookOrderRepository)
        {
            if (bookOrderRepository == null)
                throw new ArgumentNullException(nameof(bookOrderRepository));
            _bookOrderRepository = bookOrderRepository;
        }

        public void Execute(Guid bookOrderId)
        {
            Logger.Information("Execute ApproveBookOrderUseCase for Id: {Title}", bookOrderId);

            BookOrder bookorder = _bookOrderRepository.Get(bookOrderId);

            bookorder.Approve();

            _bookOrderRepository.Store(bookorder);
        }
    }
}