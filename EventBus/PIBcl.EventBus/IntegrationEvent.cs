using Newtonsoft.Json;
using System;

namespace DuCorp.EventBus
{
    public abstract class IntegrationEvent
    {
        protected IntegrationEvent(string canal, string type)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Canal = canal;
            Type = type;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createdAt, string type, string canal)
        {
            Id = id;
            CreatedAt = createdAt;
            Canal = canal;
            Type = type;
        }


        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }


        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; private set; }

        [JsonProperty(PropertyName = "createAt")]
        public DateTime CreatedAt { get; private set; }

        [JsonProperty(PropertyName = "canal")]
        public string Canal { get; private set; }
    }
}
