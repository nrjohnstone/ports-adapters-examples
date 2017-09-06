using System;

namespace Adapter.Persistence.MySql.Repositories.Dtos
{
    internal class OrderLineDto
    {
        public Guid Order_Line_Id { get; set; }
        public Guid Order_Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}