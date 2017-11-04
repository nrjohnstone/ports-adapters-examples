using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Ports.Notification;
using SimpleInjector;

namespace Adapter.Notification.Email
{
    public class NotificationAdapter
    {
        private readonly NotificationAdapterSettings _settings;
        private bool _initialized;

        public NotificationAdapter(NotificationAdapterSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
        }

        public void Initialize()
        {
            _initialized = true;
        }

        public void Register(Container container)
        {
            if (!_initialized)
                throw new InvalidOperationException("Adapter must be initialized prior to use");

            container.Register<IBookSupplierGateway>(() => new BookSupplierGateway(
                _settings.SmtpServer, _settings.SmtpPort, _settings.BookSupplierEmail));
        }

        public void Shutdown()
        {
            
        }
    }
}
