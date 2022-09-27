using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace PIBcl.EventBus.IntegrationEventLog.Services
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync();
        Task SaveEventAsync(IntegrationEvent @event);
        Task MarkEventAsPublishedAsync(Guid eventId, string partitionKey = null);
        Task MarkEventAsInProgressAsync(Guid eventId, string partitionKey = null);
        Task MarkEventAsFailedAsync(Guid eventId, string partitionKey = null);
    }
}
