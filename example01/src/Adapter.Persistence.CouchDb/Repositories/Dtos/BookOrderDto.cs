using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Adapter.Persistence.CouchDb.Repositories.Dtos
{
    internal class BookOrderDto
    {
        public BookOrderDto(string supplier, string id, BookOrderState state, IEnumerable<OrderLineDto> orderLines)
        {
            Supplier = supplier;
            _id = id;
            State = state;
            OrderLines = orderLines;
        }
        public string Supplier { get; set; }
        public string _id { get; set; }
        public string _rev { get; set; }
        public BookOrderState State { get; set; }
        public IEnumerable<OrderLineDto> OrderLines { get; set; }
    }
}