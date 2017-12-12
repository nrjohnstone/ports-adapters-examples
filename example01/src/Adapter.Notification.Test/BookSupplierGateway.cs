using System.Collections.Generic;
using Domain.Entities;
using Domain.Ports.Notification;

namespace Adapter.Notification.Test
{
    public class BookSupplierGateway : IBookSupplierGateway
    {
        public List<BookOrder> SentBookOrders { get; } = new List<BookOrder>();

        public void Send(BookOrder bookOrder)
        {
            SentBookOrders.Add(bookOrder);
        }        
    }
}