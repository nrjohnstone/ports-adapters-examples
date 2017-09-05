using System;
using AmbientContext.LogService.Serilog;
using Core.Ports.Notification;
using Core.Ports.Persistence;

namespace Core.UseCases
{
    public class SendBookOrderUseCase
    {
        public AmbientLogService Logger = new AmbientLogService();
        private readonly IBookOrderRepository _bookOrderRepository;
        private readonly IBookSupplierGateway _bookSupplierGateway;

        public SendBookOrderUseCase(IBookOrderRepository bookOrderRepository,
            IBookSupplierGateway bookSupplierGateway)
        {
            if (bookOrderRepository == null)
                throw new ArgumentNullException(nameof(bookOrderRepository));
            if (bookSupplierGateway == null) throw new ArgumentNullException(nameof(bookSupplierGateway));
            _bookOrderRepository = bookOrderRepository;
            _bookSupplierGateway = bookSupplierGateway;
        }

        public void Execute(Guid bookOrderId)
        {
            var bookOrder = _bookOrderRepository.Get(bookOrderId);

            bookOrder.Send();

            _bookSupplierGateway.Send(bookOrder);
        }

    }
}