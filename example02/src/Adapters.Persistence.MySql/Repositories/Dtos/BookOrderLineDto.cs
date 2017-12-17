using System;

namespace Adapters.Persistence.MySql.Repositories.Dtos
{
    internal class BookOrderLineDto
    {
        public Guid Order_Line_Id { get; set; }
        public Guid Order_Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}