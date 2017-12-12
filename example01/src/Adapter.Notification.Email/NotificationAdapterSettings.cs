using System;

namespace Adapter.Notification.Email
{
    public class NotificationAdapterSettings
    {        
        public NotificationAdapterSettings(string smtpServer, int smtpPort, string bookSupplierEmail)
        {
            if (smtpServer == null) throw new ArgumentNullException(nameof(smtpServer));
            if (smtpPort <= 0) throw new ArgumentOutOfRangeException(nameof(smtpPort));
            if (string.IsNullOrWhiteSpace(bookSupplierEmail))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(bookSupplierEmail));
            
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            BookSupplierEmail = bookSupplierEmail;
        }
        public string SmtpServer { get; }
        public int SmtpPort { get; }
        public string BookSupplierEmail { get; }
    }
}