using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Adapters.Persistence.EventStore.EventHandlers
{
    public abstract class BookOrderEventHandler<T> : IBookOrderEventHandler
    {
        public abstract string EventType { get; }
        protected abstract void DoHandle(T ev, BookOrderResult result);

        public bool CanHandle(RecordedEvent recordedEvent)
        {
            if (recordedEvent.EventType.Equals(EventType))
                return true;

            return false;
        }

        public void Handle(RecordedEvent recordedEvent, BookOrderResult result)
        {
            var st = Encoding.ASCII.GetString(recordedEvent.Data);
            T ev = JsonConvert.DeserializeObject<T>(st);

            DoHandle(ev, result);
        }
    }
}