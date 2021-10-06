using System;
using System.Collections.Generic;

namespace Domain.ValueObjects
{
    public class SupplierBookOrderUpdateRequest
    {
        public Guid BookOrderId { get; }
        public IReadOnlyList<SupplierBookOrderLineUpdateRequest> OrderLineUpdates { get; }

        public SupplierBookOrderUpdateRequest(Guid bookOrderId, IReadOnlyList<SupplierBookOrderLineUpdateRequest> orderLineUpdates)
        {
            BookOrderId = bookOrderId;
            OrderLineUpdates = orderLineUpdates;
        }


    }
}