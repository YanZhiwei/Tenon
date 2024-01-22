using System;

namespace Tenon.EventBus.Models
{
    [Serializable]
    public class EventBusDescriptor
    {
        public EventBusDescriptor()
        {
        }

        public EventBusDescriptor(long id, string source)
        {
            Id = id;
            EventSource = source ?? throw new ArgumentNullException(nameof(source));
        }


        public long Id { get; set; }


        public DateTime EventTime { get; set; } = DateTime.UtcNow;


        public string EventSource { get; set; } = string.Empty;


        public string EventTarget { get; set; } = string.Empty;
    }
}