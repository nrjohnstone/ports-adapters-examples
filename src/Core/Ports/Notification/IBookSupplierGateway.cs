using Core.Entities;

namespace Core.Ports.Notification
{
    public interface IBookSupplierGateway
    {
        void Send(BookOrder bookOrder);
    }
}