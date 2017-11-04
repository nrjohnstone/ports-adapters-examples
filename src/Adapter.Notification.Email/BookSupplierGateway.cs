using System;
using System.Net.Mail;
using System.Text;
using AmbientContext.LogService.Serilog;
using Domain.Entities;
using Domain.Ports.Notification;

namespace Adapter.Notification.Email
{
    internal class BookSupplierGateway : IBookSupplierGateway
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _bookSupplierEmail;
        public AmbientLogService Logger { get; } = new AmbientLogService();

        public BookSupplierGateway(string smtpServer, int smtpPort, string bookSupplierEmail)
        {
            if (string.IsNullOrWhiteSpace(smtpServer))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(smtpServer));
            if (smtpPort <= 0) throw new ArgumentOutOfRangeException(nameof(smtpPort));
            if (string.IsNullOrWhiteSpace(bookSupplierEmail))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(bookSupplierEmail));
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _bookSupplierEmail = bookSupplierEmail;
        }

        public void Send(BookOrder bookOrder)
        {
            var message = CreateMessage(bookOrder);
            SendMessage(message);
        }

        private MailMessage CreateMessage(BookOrder bookOrder)
        {
            string toAddress = _bookSupplierEmail;
            string fromAddress = "NJBookSellers@fakedomain.com";

            StringBuilder sb = new StringBuilder();

            sb.Append("Books container in order\n");

            foreach (var orderLine in bookOrder.OrderLines)
            {
                sb.Append($"Title: {orderLine.Title}\t\tPrice: {orderLine.Price}\t\tQuantity: {orderLine.Quantity}\n");
            }

            MailMessage message =
                new MailMessage(fromAddress, toAddress)
                {
                    Subject = $"Book Order for supplier {bookOrder.Supplier}.",
                    Body = sb.ToString()
                };
            return message;
        }

        private void SendMessage(MailMessage message)
        {
            SmtpClient client = new SmtpClient(_smtpServer, _smtpPort);

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Exception while sending email to book supplier gateway");
            }
        }
    }
}