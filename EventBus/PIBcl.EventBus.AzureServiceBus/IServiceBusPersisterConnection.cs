using Microsoft.Azure.ServiceBus;
using System;

namespace DuCorp.EventBus.AzureServiceBus
{
    public interface IServiceBusPersisterConnection
    {
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        ITopicClient CreateModel();
    }
}
