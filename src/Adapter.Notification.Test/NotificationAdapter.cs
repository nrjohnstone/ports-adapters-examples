using Core.Ports.Notification;
using SimpleInjector;

namespace Adapter.Notification.Test
{
    public class NotificationAdapter
    {
        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;
        }

        public void Register(Container container)
        {
            if (!_initialized)
                throw new AdpaterNotInitializedException();

            container.Register<IBookSupplierGateway, BookSupplierGateway>();
        }
    }
}