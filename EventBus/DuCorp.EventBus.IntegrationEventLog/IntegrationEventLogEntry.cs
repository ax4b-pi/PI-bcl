using Newtonsoft.Json;
using System;
using System.Linq;

namespace DuCorp.EventBus.IntegrationEventLog
{
    public class IntegrationEventLogEntry
    {
        [JsonConstructor]
        protected IntegrationEventLogEntry() { }
        public IntegrationEventLogEntry(IntegrationEvent @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreatedAt;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
        }

        [JsonProperty(PropertyName = "id")]
        public Guid EventId { get; private set; }

        [JsonProperty(PropertyName = "eventTypeName")]
        public string EventTypeName { get; private set; }        

        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        public IntegrationEvent IntegrationEvent { get; private set; }

        [JsonProperty(PropertyName = "state")]
        public EventStateEnum State { get; set; }

        [JsonProperty(PropertyName = "timesSent")]
        public int TimesSent { get; set; }

        [JsonProperty(PropertyName = "creationTime")]
        public DateTime CreationTime { get; private set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; private set; }

        internal IntegrationEventLogEntry DeserializeJsonContent()
        {
            IntegrationEvent = JsonConvert.DeserializeObject(Content, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            }
            ) as IntegrationEvent;
            return this;
        }
    }
}
