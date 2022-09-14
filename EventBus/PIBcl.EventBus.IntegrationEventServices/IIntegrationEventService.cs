using System;
using System.Threading.Tasks;

namespace DuCorp.EventBus.IntegrationEventServices
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync();
        Task PublishEventsThroughEventBusAsync(IntegrationEvent evt);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}
