using Domain.Entities;

namespace Domain.Ports.Notification
{
    public interface IBookSupplierGateway
    {
        void Send(BookOrder bookOrder);
    }
}