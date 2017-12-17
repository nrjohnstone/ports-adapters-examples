using System;
using Domain.Entities;

namespace Adapters.Persistence.MySql.Repositories.Dtos
{
    internal class BookOrderDto
    {
        public string Supplier { get; set; }
        public Guid Order_Id { get; set; }
        public BookOrderState State { get; set; }
    }
}