using System;
using Core.Entities;

namespace Core.Ports.Persistence
{
    public interface IBookOrderRepository
    {
        BookOrder Get(Guid id);
        void Store(BookOrder bookOrder);
    }
}