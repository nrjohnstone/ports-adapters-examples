using System.Collections.Generic;
using Core.Entities;
using Core.Ports.Notification;

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