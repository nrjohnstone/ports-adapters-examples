using System;
using Domain.Entities;

namespace Domain.Ports.Persistence
{
    public interface IBookOrderRepository
    {
        void Store(BookOrder bookOrder);
        BookOrder Get(Guid orderId);
    }
}