using System.Collections.Generic;
using System.Linq;
using AmbientContext.LogService.Serilog;
using Domain.Entities;
using Domain.Ports.Persistence;
using Domain.ValueObjects;

namespace Domain.UseCases
{
    public class SupplierBookOrderUpdateUseCase
    {
        private readonly IBookOrderRepository _bookOrderRepository;
        private readonly IBookOrderLineConflictRepository _bookOrderLineConflictRepository;
        public AmbientLogService Logger = new AmbientLogService();

        public SupplierBookOrderUpdateUseCase(IBookOrderRepository bookOrderRepository,
            IBookOrderLineConflictRepository bookOrderLineConflictRepository)
        {
            _bookOrderRepository = bookOrderRepository;
            _bookOrderLineConflictRepository = bookOrderLineConflictRepository;
        }

        public void Execute(SupplierBookOrderUpdateRequest supplierBookOrderUpdateRequest)
        {
            Logger.Information($"Execute {nameof(GetType)} for Id: {{BookOrderId}}", supplierBookOrderUpdateRequest.BookOrderId);

            var bookOrder = _bookOrderRepository.Get(supplierBookOrderUpdateRequest.BookOrderId);

            List<BookOrderLineConflict> conflicts =
                CalculateOrderLineConflicts(bookOrder, supplierBookOrderUpdateRequest);

            if (conflicts.Any())
            {
                _bookOrderLineConflictRepository.Store(conflicts);
            }
        }

        private List<BookOrderLineConflict> CalculateOrderLineConflicts(BookOrder bookOrder, SupplierBookOrderUpdateRequest supplierBookOrderUpdateRequest)
        {
            List<BookOrderLineConflict> conflicts = new List<BookOrderLineConflict>();

            var lineUpdates = supplierBookOrderUpdateRequest.OrderLineUpdates;

            foreach (var lineUpdate in lineUpdates)
            {
                var bookOrderLine = bookOrder.OrderLines.First(x => x.Id == lineUpdate.BookOrderLineId);

                if (bookOrderLine.Price != lineUpdate.Price)
                    conflicts.Add(BookOrderLinePriceConflict.CreateNew(bookOrder.Id,
                        bookOrderLine.Id, lineUpdate.Price));

                if (bookOrderLine.Quantity != lineUpdate.Quantity)
                    conflicts.Add(BookOrderLineQuantityConflict.CreateNew(bookOrder.Id,
                        bookOrderLine.Id, lineUpdate.Quantity));
            }

            return conflicts;
        }
    }
}