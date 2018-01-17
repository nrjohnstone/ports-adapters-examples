using EventStore.ClientAPI;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public interface IBookOrderEventHandler
    {
        bool CanHandle(RecordedEvent ev);
        void Handle(RecordedEvent recordedEvent, BookOrderResult result);
    }
}