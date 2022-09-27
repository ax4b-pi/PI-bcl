using System;
using System.Threading.Tasks;

namespace PIBcl.EventBus.IntegrationEventServices
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync();
        Task PublishEventsThroughEventBusAsync(IntegrationEvent evt);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}
