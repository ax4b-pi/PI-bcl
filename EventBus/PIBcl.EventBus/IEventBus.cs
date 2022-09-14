using System;
using System.Collections.Generic;
using System.Text;

namespace DuCorp.EventBus
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Subscribe<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
